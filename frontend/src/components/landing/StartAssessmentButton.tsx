"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { CandidateApiError, candidateApi } from "@/lib/api/client";

export function StartAssessmentButton() {
  const router = useRouter();
  const [isStarting, setIsStarting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function startAssessment() {
    setIsStarting(true);
    setError(null);

    try {
      const assessment = await candidateApi.createAssessment();
      router.push(`/assessment/${assessment.id}`);
    } catch (caught) {
      const message =
        caught instanceof CandidateApiError
          ? `${caught.message}${caught.correlationId ? ` (ID: ${caught.correlationId})` : ""}`
          : "Candidate API sa nepodarilo kontaktovať. Overte, že backend beží na porte 5158.";
      setError(message);
      setIsStarting(false);
    }
  }

  return (
    <div className="space-y-3">
      <button
        type="button"
        onClick={startAssessment}
        disabled={isStarting}
        className="inline-flex min-h-12 items-center justify-center gap-3 rounded-md bg-sky-400 px-5 py-3 text-sm font-semibold text-[#06111e] shadow-[0_12px_35px_rgba(56,189,248,0.16)] transition hover:bg-sky-300 focus:outline-none focus-visible:ring-2 focus-visible:ring-sky-200 focus-visible:ring-offset-2 focus-visible:ring-offset-[#08111f] disabled:cursor-wait disabled:opacity-65"
      >
        {isStarting ? "Vytváram assessment…" : "Spustiť candidate assessment"}
        <span aria-hidden="true">→</span>
      </button>
      {error ? (
        <p role="alert" className="max-w-xl text-sm leading-6 text-rose-200">
          {error}
        </p>
      ) : null}
    </div>
  );
}
