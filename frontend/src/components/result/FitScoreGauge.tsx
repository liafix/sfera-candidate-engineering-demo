interface FitScoreGaugeProps {
  score: number;
}

export function FitScoreGauge({ score }: FitScoreGaugeProps) {
  const boundedScore = Math.min(100, Math.max(0, score));
  const radius = 42;
  const circumference = 2 * Math.PI * radius;
  const dashOffset = circumference * (1 - boundedScore / 100);

  return (
    <div className="relative size-32 shrink-0" aria-label={`Demo fit score ${boundedScore} zo 100`}>
      <svg className="size-32 -rotate-90" viewBox="0 0 104 104" aria-hidden="true">
        <circle cx="52" cy="52" r={radius} fill="none" stroke="currentColor" strokeWidth="7" className="text-slate-200" />
        <circle
          cx="52"
          cy="52"
          r={radius}
          fill="none"
          stroke="currentColor"
          strokeWidth="7"
          strokeLinecap="round"
          strokeDasharray={circumference}
          strokeDashoffset={dashOffset}
          className="text-sky-500 transition-[stroke-dashoffset] duration-500"
        />
      </svg>
      <div className="absolute inset-0 flex flex-col items-center justify-center">
        <span className="text-3xl font-semibold tracking-[-0.04em] text-slate-950">{boundedScore}</span>
        <span className="mt-0.5 text-[10px] font-semibold uppercase tracking-[0.14em] text-slate-500">of 100</span>
      </div>
    </div>
  );
}
