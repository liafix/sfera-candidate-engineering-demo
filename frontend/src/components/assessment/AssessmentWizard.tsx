"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { CandidateApiError, candidateApi } from "@/lib/api/client";
import type { AssessmentDto, RecommendationResultDto } from "@/lib/api/types";
import { assessmentQuestions } from "@/lib/assessment/questions";
import { OptionCard } from "@/components/assessment/OptionCard";
import { RecommendationResultView } from "@/components/result/RecommendationResultView";

interface AssessmentWizardProps {
  assessmentId: string;
}

type LoadState = "loading" | "ready" | "error";

function findResumeIndex(answers: Record<string, string>): number {
  if (Object.keys(answers).length === 0) {
    return 0;
  }

  const firstUnansweredRequired = assessmentQuestions.findIndex(
    (question) => question.required && !answers[question.key],
  );

  if (firstUnansweredRequired >= 0) {
    return firstUnansweredRequired;
  }

  return Math.max(0, assessmentQuestions.length - 1);
}

export function AssessmentWizard({ assessmentId }: AssessmentWizardProps) {
  const [loadState, setLoadState] = useState<LoadState>("loading");
  const [assessment, setAssessment] = useState<AssessmentDto | null>(null);
  const [result, setResult] = useState<RecommendationResultDto | null>(null);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [draftValue, setDraftValue] = useState("");
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const currentQuestion = assessmentQuestions[currentIndex];
  const progress = Math.round(((currentIndex + 1) / assessmentQuestions.length) * 100);

  const [reloadNonce, setReloadNonce] = useState(0);

  useEffect(() => {
    let cancelled = false;

    async function hydrateAssessment() {
      try {
        const loaded = await candidateApi.getAssessment(assessmentId);

        if (cancelled) {
          return;
        }

        setAssessment(loaded);

        if (loaded.status === "resultGenerated" || loaded.status === "completed") {
          const storedResult = await candidateApi.getResult(assessmentId);

          if (cancelled) {
            return;
          }

          setResult(storedResult);
          setLoadState("ready");
          return;
        }

        const resumeIndex = findResumeIndex(loaded.answers);
        setCurrentIndex(resumeIndex);
        setDraftValue(loaded.answers[assessmentQuestions[resumeIndex].key] ?? "");
        setLoadState("ready");
      } catch (caught) {
        if (cancelled) {
          return;
        }

        setError(formatError(caught));
        setLoadState("error");
      }
    }

    void hydrateAssessment();

    return () => {
      cancelled = true;
    };
  }, [assessmentId, reloadNonce]);

  const canContinue = useMemo(() => {
    if (!currentQuestion) {
      return false;
    }

    return currentQuestion.required ? draftValue.trim().length > 0 : true;
  }, [currentQuestion, draftValue]);

  function goToIndex(nextIndex: number, latestAssessment: AssessmentDto | null = assessment) {
    const bounded = Math.min(Math.max(nextIndex, 0), assessmentQuestions.length - 1);
    setCurrentIndex(bounded);
    setDraftValue(latestAssessment?.answers[assessmentQuestions[bounded].key] ?? "");
    setError(null);
  }

  async function persistCurrentAnswer(): Promise<AssessmentDto | null> {
    if (!assessment || !currentQuestion) {
      return assessment;
    }

    const value = draftValue.trim();

    if (!currentQuestion.required && value.length === 0) {
      return assessment;
    }

    await candidateApi.saveAnswer(assessment.id, currentQuestion.key, value);

    const nextAssessment: AssessmentDto = {
      ...assessment,
      answers: {
        ...assessment.answers,
        [currentQuestion.key]: value,
      },
      status: assessment.status === "draft" ? "inProgress" : assessment.status,
    };

    setAssessment(nextAssessment);
    return nextAssessment;
  }

  async function handleContinue() {
    if (!canContinue || isSaving || !assessment) {
      return;
    }

    setIsSaving(true);
    setError(null);

    try {
      const latestAssessment = await persistCurrentAnswer();

      if (currentIndex < assessmentQuestions.length - 1) {
        goToIndex(currentIndex + 1, latestAssessment);
        return;
      }

      const evaluated = await candidateApi.evaluateAssessment(assessment.id);
      setResult(evaluated);
    } catch (caught) {
      setError(formatError(caught));
    } finally {
      setIsSaving(false);
    }
  }

  function handleBack() {
    if (isSaving || currentIndex === 0) {
      return;
    }

    goToIndex(currentIndex - 1);
  }

  if (loadState === "loading") {
    return <WizardSkeleton />;
  }

  if (loadState === "error" || !assessment) {
    return (
      <section className="rounded-xl border border-rose-200 bg-white p-6 shadow-sm sm:p-8">
        <p className="text-sm font-semibold text-rose-700">Assessment sa nepodarilo načítať.</p>
        <p className="mt-2 text-sm leading-6 text-slate-600">{error}</p>
        <div className="mt-6 flex flex-wrap gap-3">
          <button
            type="button"
            onClick={() => {
              setLoadState("loading");
              setError(null);
              setReloadNonce((value) => value + 1);
            }}
            className="rounded-md bg-slate-950 px-4 py-2.5 text-sm font-semibold text-white focus:outline-none focus-visible:ring-2 focus-visible:ring-sky-500"
          >
            Skúsiť znova
          </button>
          <Link href="/" className="rounded-md border border-slate-200 px-4 py-2.5 text-sm font-semibold text-slate-700">
            Späť na úvod
          </Link>
        </div>
      </section>
    );
  }

  if (result) {
    return <RecommendationResultView assessment={assessment} result={result} />;
  }

  if (!currentQuestion) {
    return null;
  }

  return (
    <section className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-[0_24px_80px_rgba(15,23,42,0.08)]">
      <div className="border-b border-slate-200 px-5 py-4 sm:px-8">
        <div className="flex items-center justify-between gap-4">
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-slate-500">
            Krok {currentIndex + 1} z {assessmentQuestions.length}
          </p>
          <p className="font-mono text-[11px] text-slate-500">Assessment {assessment.id.slice(0, 8)}…</p>
        </div>
        <div
          className="mt-3 h-1.5 overflow-hidden rounded-full bg-slate-100"
          role="progressbar"
          aria-valuemin={0}
          aria-valuemax={100}
          aria-valuenow={progress}
          aria-label={`Assessment progress ${progress}%`}
        >
          <div className="h-full rounded-full bg-sky-500 transition-[width] duration-300" style={{ width: `${progress}%` }} />
        </div>
      </div>

      <div className="grid lg:grid-cols-[minmax(0,1fr)_15rem]">
        <div className="px-5 py-7 sm:px-8 sm:py-9 lg:px-10">
          <h1 className="max-w-3xl text-2xl font-semibold tracking-[-0.025em] text-slate-950 sm:text-3xl">
            {currentQuestion.title}
          </h1>
          <p className="mt-3 max-w-2xl text-sm leading-6 text-slate-600 sm:text-[15px]">{currentQuestion.description}</p>

          <div className="mt-7">
            {currentQuestion.kind === "text" ? (
              <div>
                <label htmlFor={currentQuestion.key} className="sr-only">
                  {currentQuestion.title}
                </label>
                <input
                  id={currentQuestion.key}
                  type="text"
                  maxLength={200}
                  value={draftValue}
                  disabled={isSaving}
                  onChange={(event) => setDraftValue(event.target.value)}
                  placeholder={currentQuestion.placeholder}
                  className="min-h-12 w-full rounded-md border border-slate-300 bg-white px-4 py-3 text-[15px] text-slate-950 outline-none transition placeholder:text-slate-400 focus:border-sky-500 focus:ring-4 focus:ring-sky-100 disabled:bg-slate-50"
                />
                <p className="mt-2 text-xs text-slate-500">Voliteľné · maximálne 200 znakov</p>
              </div>
            ) : (
              <div className="grid gap-3" role="radiogroup" aria-label={currentQuestion.title}>
                {currentQuestion.options?.map((option) => (
                  <OptionCard
                    key={option.value}
                    name={currentQuestion.key}
                    option={option}
                    checked={draftValue === option.value}
                    disabled={isSaving}
                    onChange={setDraftValue}
                  />
                ))}
              </div>
            )}
          </div>

          {error ? (
            <div role="alert" className="mt-6 rounded-md border border-rose-200 bg-rose-50 px-4 py-3 text-sm leading-6 text-rose-800">
              {error}
            </div>
          ) : null}

          <div className="mt-8 flex flex-wrap items-center justify-between gap-3 border-t border-slate-100 pt-6">
            <button
              type="button"
              onClick={handleBack}
              disabled={currentIndex === 0 || isSaving}
              className="min-h-11 rounded-md border border-slate-200 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:border-slate-300 hover:bg-slate-50 focus:outline-none focus-visible:ring-2 focus-visible:ring-sky-500 disabled:cursor-not-allowed disabled:opacity-40"
            >
              ← Späť
            </button>
            <button
              type="button"
              onClick={() => void handleContinue()}
              disabled={!canContinue || isSaving}
              className="min-h-11 rounded-md bg-slate-950 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-slate-800 focus:outline-none focus-visible:ring-2 focus-visible:ring-sky-500 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-45"
            >
              {isSaving
                ? "Ukladám…"
                : currentIndex === assessmentQuestions.length - 1
                  ? "Vyhodnotiť assessment"
                  : "Uložiť a pokračovať →"}
            </button>
          </div>
        </div>

        <aside className="border-t border-slate-200 bg-slate-50 px-5 py-6 sm:px-8 lg:border-l lg:border-t-0 lg:px-6">
          <p className="text-xs font-semibold uppercase tracking-[0.15em] text-slate-500">Engineering notes</p>
          <dl className="mt-5 space-y-5 text-sm">
            <div>
              <dt className="font-semibold text-slate-800">Persistence</dt>
              <dd className="mt-1 leading-5 text-slate-600">Každá potvrdená odpoveď sa ukladá cez ASP.NET Core API do relačnej DB.</dd>
            </div>
            <div>
              <dt className="font-semibold text-slate-800">Determinism</dt>
              <dd className="mt-1 leading-5 text-slate-600">Vyhodnotenie nepoužíva generatívnu AI. Rovnaké vstupy = rovnaký ruleset output.</dd>
            </div>
            <div>
              <dt className="font-semibold text-slate-800">Safety</dt>
              <dd className="mt-1 leading-5 text-slate-600">Neistá kombinácia smeruje na expert review namiesto vymysleného odporúčania.</dd>
            </div>
          </dl>
        </aside>
      </div>
    </section>
  );
}

function WizardSkeleton() {
  return (
    <section className="animate-pulse rounded-xl border border-slate-200 bg-white p-6 shadow-sm sm:p-10" aria-label="Načítavam assessment">
      <div className="h-3 w-28 rounded bg-slate-200" />
      <div className="mt-7 h-8 max-w-2xl rounded bg-slate-200" />
      <div className="mt-3 h-4 max-w-xl rounded bg-slate-100" />
      <div className="mt-8 grid gap-3">
        <div className="h-24 rounded-lg bg-slate-100" />
        <div className="h-24 rounded-lg bg-slate-100" />
        <div className="h-24 rounded-lg bg-slate-100" />
      </div>
    </section>
  );
}

function formatError(caught: unknown): string {
  if (caught instanceof CandidateApiError) {
    return `${caught.message}${caught.correlationId ? ` · correlation ${caught.correlationId}` : ""}`;
  }

  return "Candidate API sa nepodarilo kontaktovať. Overte backend a NEXT_PUBLIC_API_BASE_URL.";
}
