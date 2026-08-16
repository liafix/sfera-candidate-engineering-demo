import Link from "next/link";

export function CandidateHeader() {
  return (
    <header className="border-b border-white/10 bg-[#08111f] text-white">
      <div className="mx-auto flex min-h-16 w-full max-w-7xl items-center justify-between gap-6 px-5 py-3 sm:px-8 lg:px-10">
        <Link href="/" className="group inline-flex items-center gap-3 rounded-sm focus:outline-none focus-visible:ring-2 focus-visible:ring-sky-300">
          <span className="grid size-8 place-items-center border border-sky-300/30 bg-sky-300/10 font-mono text-xs font-semibold text-sky-200 transition group-hover:border-sky-300/60">
            S/
          </span>
          <span className="leading-tight">
            <span className="block text-sm font-semibold tracking-tight">SFÉRA Candidate Engineering Demo</span>
            <span className="block text-[11px] text-slate-400">Energy Solution &amp; ROI Configurator</span>
          </span>
        </Link>
        <span className="hidden text-xs text-slate-400 sm:block">Independent candidate project · synthetic data only</span>
      </div>
    </header>
  );
}
