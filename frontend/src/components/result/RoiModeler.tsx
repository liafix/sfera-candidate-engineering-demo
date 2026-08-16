"use client";

import { useEffect, useMemo, useState } from "react";
import { CandidateApiError, candidateApi } from "@/lib/api/client";
import type { RoiScenarioDto } from "@/lib/api/types";
import {
  getScenarioPreset,
  roiScenarioPresets,
  type RoiAssumptions,
  type RoiScenarioName,
} from "@/lib/roi/scenarios";
import { MetricCard } from "@/components/result/MetricCard";

interface RoiModelerProps {
  assessmentId: string;
}

type LoadState = "idle" | "loading" | "ready" | "error";

type AssumptionKey = keyof RoiAssumptions;

const assumptionFields: readonly {
  key: AssumptionKey;
  label: string;
  suffix: string;
  help: string;
  step: number;
}[] = [
  {
    key: "casesPerMonth",
    label: "Cases per month",
    suffix: "prípadov",
    help: "Modelový mesačný objem relevantných prípadov.",
    step: 10,
  },
  {
    key: "minutesSavedPerCase",
    label: "Minutes saved per case",
    suffix: "min",
    help: "Ilustračná časová úspora na jeden prípad.",
    step: 5,
  },
  {
    key: "loadedHourlyCost",
    label: "Loaded hourly cost",
    suffix: "€ / h",
    help: "Modelový plný hodinový náklad použitý vo výpočte.",
    step: 5,
  },
  {
    key: "annualOperatingCost",
    label: "Annual operating cost",
    suffix: "€ / rok",
    help: "Ilustračný ročný prevádzkový náklad riešenia.",
    step: 500,
  },
  {
    key: "implementationCost",
    label: "Implementation cost",
    suffix: "€",
    help: "Ilustračná jednorazová implementačná investícia.",
    step: 1_000,
  },
] as const;

export function RoiModeler({ assessmentId }: RoiModelerProps) {
  const [scenarioName, setScenarioName] = useState<RoiScenarioName>("reference");
  const [assumptions, setAssumptions] = useState<RoiAssumptions>(() => ({
    ...getScenarioPreset("reference").assumptions,
  }));
  const [result, setResult] = useState<RoiScenarioDto | null>(null);
  const [lastSubmittedAssumptions, setLastSubmittedAssumptions] = useState<RoiAssumptions | null>(null);
  const [loadState, setLoadState] = useState<LoadState>("idle");
  const [error, setError] = useState<string | null>(null);

  const selectedPreset = useMemo(() => getScenarioPreset(scenarioName), [scenarioName]);
  const hasUnsavedChanges = useMemo(() => {
    if (!lastSubmittedAssumptions) {
      return false;
    }

    return (Object.keys(assumptions) as AssumptionKey[]).some(
      (key) => assumptions[key] !== lastSubmittedAssumptions[key],
    );
  }, [assumptions, lastSubmittedAssumptions]);

  useEffect(() => {
    let cancelled = false;

    async function calculateReferenceScenario() {
      setLoadState("loading");
      setError(null);

      try {
        const response = await candidateApi.calculateRoi(assessmentId, {
          scenarioName: "reference",
          ...getScenarioPreset("reference").assumptions,
        });

        if (cancelled) {
          return;
        }

        setResult(response);
        setLastSubmittedAssumptions({ ...getScenarioPreset("reference").assumptions });
        setLoadState("ready");
      } catch (caught) {
        if (cancelled) {
          return;
        }

        setError(formatError(caught));
        setLoadState("error");
      }
    }

    void calculateReferenceScenario();

    return () => {
      cancelled = true;
    };
  }, [assessmentId]);

  async function applyScenario(nextName: RoiScenarioName) {
    if (loadState === "loading") {
      return;
    }

    const preset = getScenarioPreset(nextName);
    const nextAssumptions = { ...preset.assumptions };

    setScenarioName(nextName);
    setAssumptions(nextAssumptions);
    await calculate(nextName, nextAssumptions);
  }

  async function calculate(name = scenarioName, input = assumptions) {
    setLoadState("loading");
    setError(null);

    try {
      const response = await candidateApi.calculateRoi(assessmentId, {
        scenarioName: name,
        ...input,
      });

      setResult(response);
      setLastSubmittedAssumptions({ ...input });
      setLoadState("ready");
    } catch (caught) {
      setError(formatError(caught));
      setLoadState("error");
    }
  }

  function updateAssumption(key: AssumptionKey, value: number) {
    setAssumptions((current) => ({
      ...current,
      [key]: Number.isFinite(value) && value >= 0 ? value : 0,
    }));
  }

  return (
    <section id="business-case" className="border-t border-slate-200 bg-slate-50/70">
      <div className="px-5 py-8 sm:px-8 sm:py-10 lg:px-10 lg:py-12">
        <div className="flex flex-col gap-6 border-b border-slate-200 pb-8 lg:flex-row lg:items-end lg:justify-between">
          <div className="max-w-2xl">
            <p className="text-xs font-semibold uppercase tracking-[0.15em] text-slate-500">Transparent scenario model</p>
            <h2 className="mt-3 text-2xl font-semibold tracking-[-0.03em] text-slate-950 sm:text-3xl">
              Business case bez skrytých predpokladov
            </h2>
            <p className="mt-3 text-sm leading-6 text-slate-600">
              Každé číslo nižšie vychádza iba z viditeľných syntetických vstupov a z deterministického C# ROI Calculatora.
              Nejde o forecast, cenovú ponuku ani garantovaný finančný výsledok.
            </p>
          </div>
          <div className="max-w-sm text-xs leading-5 text-slate-500">
            Server-side calculation · EF Core persistence · rovnaký scenario name sa prepočíta bez vytvorenia duplicitného záznamu.
          </div>
        </div>

        <div className="mt-8 grid gap-8 xl:grid-cols-[20rem_minmax(0,1fr)]">
          <div>
            <div className="border border-slate-200 bg-white">
              <div className="border-b border-slate-200 px-4 py-4">
                <p className="text-xs font-semibold uppercase tracking-[0.13em] text-slate-500">Scenario</p>
              </div>
              <div className="divide-y divide-slate-200">
                {roiScenarioPresets.map((scenario) => {
                  const selected = scenario.name === scenarioName;

                  return (
                    <button
                      key={scenario.name}
                      type="button"
                      aria-pressed={selected}
                      disabled={loadState === "loading"}
                      onClick={() => void applyScenario(scenario.name)}
                      className={`w-full px-4 py-4 text-left transition focus:outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-sky-500 disabled:cursor-wait disabled:opacity-70 ${
                        selected ? "bg-slate-950 text-white" : "bg-white text-slate-800 hover:bg-slate-50"
                      }`}
                    >
                      <span className="flex items-center justify-between gap-3">
                        <span className="text-sm font-semibold">{scenario.label}</span>
                        <span className={selected ? "font-mono text-[10px] text-slate-400" : "font-mono text-[10px] text-slate-400"}>
                          {scenario.name}
                        </span>
                      </span>
                      <span className={selected ? "mt-1.5 block text-xs leading-5 text-slate-300" : "mt-1.5 block text-xs leading-5 text-slate-500"}>
                        {scenario.description}
                      </span>
                    </button>
                  );
                })}
              </div>
            </div>

            <div className="mt-4 border border-amber-200 bg-amber-50 px-4 py-4 text-xs leading-5 text-amber-950">
              <strong>Model only.</strong> Presety sú zámerne syntetické. V produkcii by všetky assumptions musel schváliť business owner a finance/product stakeholder.
            </div>
          </div>

          <div className="min-w-0">
            <div className="grid gap-px border border-slate-200 bg-slate-200 sm:grid-cols-2 xl:grid-cols-4">
              <MetricCard
                label="Annual hours saved"
                value={result ? formatNumber(result.annualHoursSaved, 0) : "—"}
                detail="cases/year × minutes saved ÷ 60"
              />
              <MetricCard
                label="Annual time value"
                value={result ? formatCurrency(result.annualTimeValue) : "—"}
                detail="annual hours × loaded hourly cost"
              />
              <MetricCard
                label="Annual net benefit"
                value={result ? formatCurrency(result.annualNetBenefit) : "—"}
                detail="time value − annual operating cost"
                emphasis
              />
              <MetricCard
                label="Simple payback"
                value={result ? formatPayback(result.simplePaybackMonths) : "—"}
                detail="implementation cost ÷ annual net benefit × 12"
              />
            </div>

            <div className="mt-6 border border-slate-200 bg-white">
              <div className="flex flex-col gap-2 border-b border-slate-200 px-5 py-4 sm:flex-row sm:items-center sm:justify-between">
                <div>
                  <p className="text-sm font-semibold text-slate-950">Scenario assumptions</p>
                  <p className="mt-1 text-xs text-slate-500">Uprav vstup a pošli nový deterministický výpočet na backend.</p>
                </div>
                <p className="font-mono text-[10px] uppercase tracking-[0.1em] text-slate-400">
                  {selectedPreset.label} · synthetic
                </p>
              </div>

              <div className="grid md:grid-cols-2">
                {assumptionFields.map((field, index) => (
                  <label
                    key={field.key}
                    className={`block px-5 py-5 ${index % 2 === 0 ? "md:border-r md:border-slate-200" : ""} ${index >= 2 ? "border-t border-slate-200" : index === 1 ? "border-t border-slate-200 md:border-t-0" : ""}`}
                  >
                    <span className="flex items-center justify-between gap-4">
                      <span className="text-xs font-semibold uppercase tracking-[0.1em] text-slate-500">{field.label}</span>
                      <span className="font-mono text-[10px] text-slate-400">{field.suffix}</span>
                    </span>
                    <input
                      type="number"
                      min={0}
                      step={field.step}
                      value={assumptions[field.key]}
                      disabled={loadState === "loading"}
                      onChange={(event) => updateAssumption(field.key, Number(event.target.value))}
                      className="mt-3 min-h-11 w-full border-0 border-b border-slate-300 bg-transparent px-0 py-2 text-xl font-semibold tracking-[-0.02em] text-slate-950 outline-none transition focus:border-sky-500 disabled:opacity-60"
                    />
                    <span className="mt-2 block text-xs leading-5 text-slate-500">{field.help}</span>
                  </label>
                ))}
              </div>

              <div className="flex flex-col gap-4 border-t border-slate-200 px-5 py-5 sm:flex-row sm:items-center sm:justify-between">
                <div aria-live="polite" className="min-h-5 text-xs leading-5 text-slate-500">
                  {loadState === "loading" ? "Prepočítavam na ASP.NET Core API…" : null}
                  {loadState === "ready" && result && hasUnsavedChanges
                    ? "Vstupy boli zmenené — metriky zatiaľ zobrazujú posledný potvrdený server result."
                    : null}
                  {loadState === "ready" && result && !hasUnsavedChanges
                    ? `Server result updated ${formatTimestamp(result.updatedAt)}.`
                    : null}
                  {loadState === "error" ? <span className="text-rose-700">{error}</span> : null}
                </div>
                <button
                  type="button"
                  disabled={loadState === "loading"}
                  onClick={() => void calculate()}
                  className="min-h-11 shrink-0 bg-slate-950 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-slate-800 focus:outline-none focus-visible:ring-2 focus-visible:ring-sky-500 focus-visible:ring-offset-2 disabled:cursor-wait disabled:opacity-55"
                >
                  {loadState === "loading" ? "Prepočítavam…" : "Prepočítať model"}
                </button>
              </div>
            </div>

            <div className="mt-6 grid gap-5 border border-slate-200 bg-white px-5 py-5 text-xs leading-5 text-slate-600 md:grid-cols-3">
              <div>
                <p className="font-semibold text-slate-900">Formula 01</p>
                <p className="mt-1 font-mono text-[11px] text-slate-500">cases/year = cases/month × 12</p>
              </div>
              <div>
                <p className="font-semibold text-slate-900">Formula 02</p>
                <p className="mt-1 font-mono text-[11px] text-slate-500">time value = hours saved × hourly cost</p>
              </div>
              <div>
                <p className="font-semibold text-slate-900">Formula 03</p>
                <p className="mt-1 font-mono text-[11px] text-slate-500">net benefit = time value − run cost</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat("sk-SK", {
    style: "currency",
    currency: "EUR",
    maximumFractionDigits: 0,
  }).format(value);
}

function formatNumber(value: number, maximumFractionDigits = 2): string {
  return new Intl.NumberFormat("sk-SK", {
    maximumFractionDigits,
  }).format(value);
}

function formatPayback(value: number | null): string {
  if (value === null) {
    return "Not reached";
  }

  if (value === 0) {
    return "0 mes.";
  }

  return `${formatNumber(value, 1)} mes.`;
}

function formatTimestamp(value: string): string {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "now";
  }

  return new Intl.DateTimeFormat("sk-SK", {
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  }).format(date);
}

function formatError(caught: unknown): string {
  if (caught instanceof CandidateApiError) {
    return `${caught.message}${caught.correlationId ? ` · correlation ${caught.correlationId}` : ""}`;
  }

  return "ROI API sa nepodarilo kontaktovať. Overte backend a NEXT_PUBLIC_API_BASE_URL.";
}
