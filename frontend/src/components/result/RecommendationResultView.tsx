import Link from "next/link";
import type { AssessmentDto, RecommendationResultDto } from "@/lib/api/types";
import { FitScoreGauge } from "@/components/result/FitScoreGauge";
import { RoiModeler } from "@/components/result/RoiModeler";

interface RecommendationResultViewProps {
  assessment: AssessmentDto;
  result: RecommendationResultDto;
}

export function RecommendationResultView({ assessment, result }: RecommendationResultViewProps) {
  const organizationName = assessment.answers.organizationName?.trim() || "Synthetic organization";

  return (
    <section className="overflow-hidden border border-slate-200 bg-white shadow-[0_24px_80px_rgba(15,23,42,0.08)]">
      <div className="border-b border-slate-200 bg-slate-950 px-5 py-4 text-white sm:px-8">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div className="flex items-center gap-3">
            <span className="size-2 rounded-full bg-emerald-400" aria-hidden="true" />
            <p className="text-xs font-semibold uppercase tracking-[0.15em] text-slate-200">Assessment evaluated</p>
          </div>
          <p className="font-mono text-[10px] uppercase tracking-[0.12em] text-slate-400">
            {result.ruleSetVersion}
          </p>
        </div>
      </div>

      <div className="grid gap-0 xl:grid-cols-[minmax(0,1fr)_21rem]">
        <div className="px-5 py-8 sm:px-8 sm:py-10 lg:px-10 lg:py-12">
          <div className="max-w-3xl">
            <p className="text-xs font-semibold uppercase tracking-[0.14em] text-slate-500">Suggested solution pathway</p>
            <h1 className="mt-3 text-3xl font-semibold tracking-[-0.04em] text-slate-950 sm:text-4xl">
              {result.displayName}
            </h1>
            <p className="mt-4 max-w-2xl text-sm leading-6 text-slate-600 sm:text-[15px]">
              Výsledok vznikol z verejne opísaných capability oblastí a z candidate-demo rulesetu. Nie je to oficiálne produktové odporúčanie SFÉRY ani náhrada odborného discovery.
            </p>
          </div>

          <div className="mt-8 grid gap-px border border-slate-200 bg-slate-200 md:grid-cols-3">
            <SummaryCell label="Organization" value={organizationName} />
            <SummaryCell label="Product code" value={result.productCode} mono />
            <SummaryCell label="Expert review" value={result.requiresExpertReview ? "Required" : "Still recommended"} />
          </div>

          <div className="mt-9 grid gap-8 lg:grid-cols-[minmax(0,1fr)_15rem]">
            <div>
              <div className="flex items-center justify-between gap-4 border-b border-slate-200 pb-3">
                <h2 className="text-sm font-semibold text-slate-950">Why this pathway</h2>
                <span className="font-mono text-[10px] uppercase tracking-[0.1em] text-slate-400">explainable output</span>
              </div>
              <ol className="divide-y divide-slate-100">
                {result.reasons.map((reason, index) => (
                  <li key={`${index}-${reason}`} className="grid grid-cols-[2.25rem_minmax(0,1fr)] gap-3 py-4 text-sm leading-6 text-slate-600">
                    <span className="font-mono text-xs font-semibold text-sky-600">{String(index + 1).padStart(2, "0")}</span>
                    <span>{reason}</span>
                  </li>
                ))}
              </ol>
            </div>

            <div className="border-l-2 border-amber-400 bg-amber-50 px-4 py-4 text-sm leading-6 text-amber-950 lg:self-start">
              <p className="font-semibold">Expert validation required</p>
              <p className="mt-2 text-xs leading-5">
                Candidate demo zámerne končí pri vysvetliteľnom pathway. Finálny product fit, technická architektúra a obchodné podmienky musí validovať domain specialist.
              </p>
            </div>
          </div>
        </div>

        <aside className="border-t border-slate-200 bg-slate-50 px-5 py-8 sm:px-8 xl:border-l xl:border-t-0 xl:px-7">
          <FitScoreGauge score={result.fitScore} />
          <div className="mt-6">
            <p className="text-sm font-semibold text-slate-950">Demo fit score</p>
            <p className="mt-2 text-xs leading-5 text-slate-500">
              Interný score rulesetu. Nie je to pravdepodobnosť úspechu, obchodný rating ani oficiálna metrika SFÉRY.
            </p>
          </div>

          <dl className="mt-7 divide-y divide-slate-200 border-y border-slate-200 text-sm">
            <MetadataRow label="Status" value={formatStatus(result.status)} />
            <MetadataRow label="Assessment" value={`${assessment.id.slice(0, 8)}…`} mono />
            <MetadataRow label="Recommendation" value={`${result.recommendationId.slice(0, 8)}…`} mono />
            <MetadataRow label="Evaluated" value={formatDateTime(result.createdAt)} />
          </dl>

          <a
            href="#business-case"
            className="mt-7 flex min-h-11 w-full items-center justify-between bg-slate-950 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-slate-800 focus:outline-none focus-visible:ring-2 focus-visible:ring-sky-500 focus-visible:ring-offset-2"
          >
            Explore business case
            <ArrowDownIcon />
          </a>
          <Link
            href="/"
            className="mt-2 flex min-h-11 w-full items-center justify-center border border-slate-300 bg-white px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-slate-100 focus:outline-none focus-visible:ring-2 focus-visible:ring-sky-500"
          >
            Nový assessment
          </Link>
        </aside>
      </div>

      <RoiModeler assessmentId={assessment.id} />
    </section>
  );
}

function SummaryCell({ label, value, mono = false }: { label: string; value: string; mono?: boolean }) {
  return (
    <div className="bg-slate-50 px-4 py-4 sm:px-5">
      <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-slate-500">{label}</p>
      <p className={`mt-2 text-sm font-semibold text-slate-900 ${mono ? "font-mono text-xs" : ""}`}>{value}</p>
    </div>
  );
}

function MetadataRow({ label, value, mono = false }: { label: string; value: string; mono?: boolean }) {
  return (
    <div className="py-3.5">
      <dt className="text-[10px] font-semibold uppercase tracking-[0.12em] text-slate-500">{label}</dt>
      <dd className={`mt-1.5 text-xs text-slate-700 ${mono ? "font-mono" : ""}`}>{value}</dd>
    </div>
  );
}

function formatStatus(value: string): string {
  return value
    .replace(/([a-z])([A-Z])/g, "$1 $2")
    .replace(/^./, (character) => character.toUpperCase());
}

function formatDateTime(value: string): string {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return value;
  }

  return new Intl.DateTimeFormat("sk-SK", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(date);
}

function ArrowDownIcon() {
  return (
    <svg viewBox="0 0 20 20" fill="none" className="size-4" aria-hidden="true">
      <path d="M10 4v11m0 0 4-4m-4 4-4-4" stroke="currentColor" strokeWidth="1.7" strokeLinecap="round" strokeLinejoin="round" />
    </svg>
  );
}
