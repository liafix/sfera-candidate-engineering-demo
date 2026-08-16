export type RoiScenarioName = "conservative" | "reference" | "growth";

export interface RoiAssumptions {
  casesPerMonth: number;
  minutesSavedPerCase: number;
  loadedHourlyCost: number;
  annualOperatingCost: number;
  implementationCost: number;
}

export interface RoiScenarioPreset {
  name: RoiScenarioName;
  label: string;
  description: string;
  assumptions: RoiAssumptions;
}

export const roiScenarioPresets: readonly RoiScenarioPreset[] = [
  {
    name: "conservative",
    label: "Conservative",
    description: "Nižší objem a nižšia modelová časová úspora.",
    assumptions: {
      casesPerMonth: 70,
      minutesSavedPerCase: 20,
      loadedHourlyCost: 35,
      annualOperatingCost: 7_000,
      implementationCost: 14_000,
    },
  },
  {
    name: "reference",
    label: "Reference",
    description: "Referenčný syntetický scenár pre interview walkthrough.",
    assumptions: {
      casesPerMonth: 100,
      minutesSavedPerCase: 30,
      loadedHourlyCost: 40,
      annualOperatingCost: 6_000,
      implementationCost: 12_000,
    },
  },
  {
    name: "growth",
    label: "Growth",
    description: "Vyšší modelový objem a väčšia časová úspora.",
    assumptions: {
      casesPerMonth: 160,
      minutesSavedPerCase: 40,
      loadedHourlyCost: 45,
      annualOperatingCost: 7_000,
      implementationCost: 14_000,
    },
  },
] as const;

export function getScenarioPreset(name: RoiScenarioName): RoiScenarioPreset {
  const preset = roiScenarioPresets.find((candidate) => candidate.name === name);

  if (!preset) {
    throw new Error(`Unsupported ROI scenario: ${name}`);
  }

  return preset;
}
