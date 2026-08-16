export type QuestionKind = "text" | "single";

export interface AssessmentOption {
  value: string;
  label: string;
  description: string;
}

export interface AssessmentQuestion {
  key: string;
  title: string;
  description: string;
  kind: QuestionKind;
  required: boolean;
  placeholder?: string;
  options?: AssessmentOption[];
}

export const assessmentQuestions: readonly AssessmentQuestion[] = [
  {
    key: "organizationName",
    title: "Ako môžeme označiť modelovú organizáciu?",
    description:
      "Voliteľný názov slúži iba na orientáciu v syntetickom candidate-demo flow. Nepoužívajte reálne citlivé údaje.",
    kind: "text",
    required: false,
    placeholder: "Napr. Demo Energy Trading SK",
  },
  {
    key: "participantType",
    title: "Aký typ organizácie najlepšie vystihuje modelový scenár?",
    description:
      "Kategórie sú zjednodušený demo model. Nejde o internú segmentáciu SFÉRY.",
    kind: "single",
    required: true,
    options: [
      {
        value: "trader_or_supplier",
        label: "Obchodník alebo dodávateľ",
        description: "Modelový účastník rieši obchodovanie, dodávku alebo správu kontraktov.",
      },
      {
        value: "distribution_operator",
        label: "Distribučný operátor",
        description: "Modelový scenár je orientovaný na distribučnú prevádzku.",
      },
      {
        value: "market_operator",
        label: "Organizátor trhu",
        description: "Modelový scenár sa týka procesov organizátora trhu.",
      },
      {
        value: "industrial_consumer",
        label: "Priemyselný odberateľ",
        description: "Modelový scenár predstavuje väčšieho koncového odberateľa.",
      },
      {
        value: "other",
        label: "Iný typ organizácie",
        description: "Demo pravidlá nemusia vedieť túto kombináciu jednoznačne priradiť.",
      },
    ],
  },
  {
    key: "primaryNeed",
    title: "Ktorá potreba je v tomto scenári najdôležitejšia?",
    description:
      "Výber ovplyvňuje iba transparentný candidate-demo ruleset a nie je záväzným produktovým odporúčaním.",
    kind: "single",
    required: true,
    options: [
      {
        value: "trading_and_supply",
        label: "Trading a supply",
        description: "Obchodovanie, dodávka, plánovanie alebo súvisiaca správa obchodných procesov.",
      },
      {
        value: "distribution",
        label: "Distribúcia",
        description: "Prevádzkové a dátové potreby distribučnej organizácie.",
      },
      {
        value: "market_operations",
        label: "Market operations",
        description: "Procesy súvisiace s fungovaním a podporou trhu.",
      },
      {
        value: "compliance_reporting",
        label: "Compliance a reporting",
        description: "Regulačné, reportovacie alebo kontrolné potreby.",
      },
      {
        value: "other",
        label: "Iná potreba",
        description: "Výsledok bude bezpečne smerovať na odbornú konzultáciu, ak pravidlá nestačia.",
      },
    ],
  },
  {
    key: "managesWholesaleContracts",
    title: "Pracuje modelový scenár s veľkoobchodnými energetickými kontraktmi?",
    description:
      "Odpoveď je vstupom do deterministického pravidla. Candidate demo nehodnotí kvalitu ani cenu kontraktov.",
    kind: "single",
    required: true,
    options: [
      {
        value: "true",
        label: "Áno",
        description: "Veľkoobchodné kontrakty sú súčasťou modelového procesu.",
      },
      {
        value: "false",
        label: "Nie",
        description: "Veľkoobchodné kontrakty nie sú jadrom tohto modelového scenára.",
      },
    ],
  },
  {
    key: "needsTradingOrPlanningSupport",
    title: "Je potrebná podpora tradingu alebo plánovania?",
    description:
      "Posledný required vstup pre candidate-demo vyhodnotenie. Rovnaké vstupy vždy vedú k rovnakému výsledku.",
    kind: "single",
    required: true,
    options: [
      {
        value: "true",
        label: "Áno",
        description: "Trading alebo plánovanie je súčasťou modelovej potreby.",
      },
      {
        value: "false",
        label: "Nie",
        description: "Táto funkčná oblasť nie je v modelovom scenári prioritou.",
      },
    ],
  },
] as const;

export const requiredQuestionKeys = new Set(
  assessmentQuestions.filter((question) => question.required).map((question) => question.key),
);
