import { StartAssessmentButton } from "@/components/landing/StartAssessmentButton";

const engineeringSignals = [
  ["Frontend", "Next.js + React + TypeScript"],
  ["API", "ASP.NET Core · REST/JSON"],
  ["Rules", "Deterministic + versioned"],
  ["Persistence", "EF Core + relational SQLite"],
] as const;

export default function HomePage() {
  return (
    <main>
      <section className="relative overflow-hidden bg-[#08111f] text-white">
        <div className="absolute inset-0 candidate-grid opacity-25" aria-hidden="true" />
        <div className="relative mx-auto grid min-h-[calc(100vh-7.5rem)] w-full max-w-7xl items-center gap-12 px-5 py-14 sm:px-8 sm:py-18 lg:grid-cols-[minmax(0,1.1fr)_minmax(23rem,0.9fr)] lg:px-10 lg:py-20">
          <div>
            <h1 className="max-w-4xl text-4xl font-semibold tracking-[-0.045em] text-white sm:text-5xl lg:text-[4.25rem] lg:leading-[1.02]">
              Explainable energy solution discovery, built as an engineering exercise.
            </h1>
            <p className="mt-6 max-w-2xl text-base leading-7 text-slate-300 sm:text-lg">
              Funkčný vertical slice candidate projektu: assessment workflow, versioned business rules, ASP.NET Core API a relačná persistence. Žiadne predstieranie interných dát ani „AI magic“ rozhodovanie.
            </p>
            <div className="mt-8">
              <StartAssessmentButton />
            </div>
            <p className="mt-5 max-w-xl text-xs leading-5 text-slate-500">
              Demo nepoužíva reálne zákaznícke dáta. Zadané hodnoty majú byť syntetické a slúžia iba na technickú ukážku.
            </p>
          </div>

          <div className="relative">
            <div className="absolute -inset-8 bg-sky-400/5 blur-3xl" aria-hidden="true" />
            <div className="relative overflow-hidden rounded-xl border border-white/12 bg-white/[0.045] shadow-[0_28px_90px_rgba(0,0,0,0.3)] backdrop-blur-sm">
              <div className="flex items-center justify-between border-b border-white/10 px-5 py-4">
                <div>
                  <p className="text-sm font-semibold text-white">Candidate system boundary</p>
                  <p className="mt-0.5 text-xs text-slate-400">Phase 5 · result → ROI API → domain → DB</p>
                </div>
                <span className="font-mono text-[11px] text-emerald-300">LOCAL / SYNTHETIC</span>
              </div>

              <div className="divide-y divide-white/10 px-5">
                {engineeringSignals.map(([label, value], index) => (
                  <div key={label} className="grid grid-cols-[7rem_1fr_auto] items-center gap-4 py-4">
                    <span className="text-xs uppercase tracking-[0.12em] text-slate-500">{label}</span>
                    <span className="text-sm text-slate-200">{value}</span>
                    <span className="font-mono text-[10px] text-sky-300">0{index + 1}</span>
                  </div>
                ))}
              </div>

              <div className="grid grid-cols-3 border-t border-white/10 bg-black/10 text-center">
                <div className="border-r border-white/10 px-3 py-4">
                  <p className="font-mono text-xs text-emerald-300">IDEMPOTENT</p>
                  <p className="mt-1 text-[10px] text-slate-500">evaluation</p>
                </div>
                <div className="border-r border-white/10 px-3 py-4">
                  <p className="font-mono text-xs text-emerald-300">AUDITABLE</p>
                  <p className="mt-1 text-[10px] text-slate-500">events</p>
                </div>
                <div className="px-3 py-4">
                  <p className="font-mono text-xs text-emerald-300">EXPLAINABLE</p>
                  <p className="mt-1 text-[10px] text-slate-500">reasons</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="border-b border-slate-200 bg-white">
        <div className="mx-auto grid w-full max-w-7xl gap-10 px-5 py-16 sm:px-8 lg:grid-cols-[0.72fr_1.28fr] lg:px-10 lg:py-20">
          <div>
            <h2 className="text-2xl font-semibold tracking-[-0.03em] text-slate-950 sm:text-3xl">Čo má demo dokázať</h2>
            <p className="mt-4 max-w-md text-sm leading-7 text-slate-600">
              Nie šírku funkcií, ale schopnosť navrhnúť workflow, oddeliť vrstvy systému a obhájiť rozhodnutia v kóde.
            </p>
          </div>
          <div className="grid gap-px overflow-hidden rounded-xl border border-slate-200 bg-slate-200 sm:grid-cols-2">
            {[
              ["01", "Workflow", "Používateľ prejde assessmentom a každá potvrdená odpoveď sa persistuje cez API."],
              ["02", "Rules", "Recommendation Engine je deterministický, verzovaný a vracia explicitné dôvody."],
              ["03", "Boundaries", "Frontend neobsahuje doménovú logiku; API orchestration ostáva v Application vrstve."],
              ["04", "Failure modes", "Neistý vstup neprodukuje vymyslený produkt — systém vyžiada expert review."],
            ].map(([number, title, body]) => (
              <article key={number} className="bg-white p-6 sm:p-7">
                <p className="font-mono text-xs text-sky-700">{number}</p>
                <h3 className="mt-5 text-base font-semibold text-slate-950">{title}</h3>
                <p className="mt-2 text-sm leading-6 text-slate-600">{body}</p>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="bg-slate-50">
        <div className="mx-auto w-full max-w-7xl px-5 py-14 sm:px-8 lg:px-10 lg:py-16">
          <div className="grid gap-8 lg:grid-cols-[1fr_1fr]">
            <div className="rounded-xl border border-slate-200 bg-white p-6 sm:p-8">
              <h2 className="text-lg font-semibold text-slate-950">What this is</h2>
              <ul className="mt-5 space-y-3 text-sm leading-6 text-slate-600">
                <li>Independent candidate engineering exercise.</li>
                <li>Functional frontend connected to a real candidate-demo API.</li>
                <li>Public context + explicitly synthetic assumptions.</li>
                <li>Small vertical slice designed to be technically defensible.</li>
              </ul>
            </div>
            <div className="rounded-xl border border-slate-200 bg-white p-6 sm:p-8">
              <h2 className="text-lg font-semibold text-slate-950">What this is not</h2>
              <ul className="mt-5 space-y-3 text-sm leading-6 text-slate-600">
                <li>Official SFÉRA product, design or internal architecture.</li>
                <li>Production energy-system integration.</li>
                <li>Binding product, price or ROI recommendation.</li>
                <li>Claim of access to confidential customer or company data.</li>
              </ul>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}
