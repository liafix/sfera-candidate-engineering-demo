import type { AssessmentOption } from "@/lib/assessment/questions";

interface OptionCardProps {
  name: string;
  option: AssessmentOption;
  checked: boolean;
  disabled: boolean;
  onChange: (value: string) => void;
}

export function OptionCard({ name, option, checked, disabled, onChange }: OptionCardProps) {
  return (
    <label
      className={`group grid cursor-pointer grid-cols-[auto_1fr] gap-4 rounded-lg border p-4 transition sm:p-5 ${
        checked
          ? "border-sky-500 bg-sky-50 shadow-[0_8px_24px_rgba(2,132,199,0.08)]"
          : "border-slate-200 bg-white hover:border-slate-300 hover:bg-slate-50"
      } ${disabled ? "cursor-not-allowed opacity-60" : ""}`}
    >
      <input
        type="radio"
        name={name}
        value={option.value}
        checked={checked}
        disabled={disabled}
        onChange={() => onChange(option.value)}
        className="mt-1 size-4 accent-sky-600"
      />
      <span>
        <span className="block text-sm font-semibold text-slate-950 sm:text-[15px]">{option.label}</span>
        <span className="mt-1 block text-sm leading-6 text-slate-600">{option.description}</span>
      </span>
    </label>
  );
}
