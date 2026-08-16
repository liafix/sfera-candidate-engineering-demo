interface MetricCardProps {
  label: string;
  value: string;
  detail: string;
  emphasis?: boolean;
}

export function MetricCard({ label, value, detail, emphasis = false }: MetricCardProps) {
  return (
    <div
      className={
        emphasis
          ? "border-t-2 border-sky-500 bg-slate-950 px-5 py-5 text-white"
          : "border-t border-slate-200 bg-white px-5 py-5"
      }
    >
      <p className={emphasis ? "text-xs font-semibold uppercase tracking-[0.13em] text-slate-400" : "text-xs font-semibold uppercase tracking-[0.13em] text-slate-500"}>
        {label}
      </p>
      <p className={emphasis ? "mt-3 text-2xl font-semibold tracking-[-0.03em] text-white" : "mt-3 text-2xl font-semibold tracking-[-0.03em] text-slate-950"}>
        {value}
      </p>
      <p className={emphasis ? "mt-2 text-xs leading-5 text-slate-400" : "mt-2 text-xs leading-5 text-slate-500"}>{detail}</p>
    </div>
  );
}
