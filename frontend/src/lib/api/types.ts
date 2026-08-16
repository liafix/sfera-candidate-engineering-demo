export type AssessmentStatus =
  | "draft"
  | "inProgress"
  | "readyForResult"
  | "resultGenerated"
  | "completed"
  | "abandoned"
  | "expired"
  | "deleted";

export interface AssessmentDto {
  id: string;
  status: AssessmentStatus;
  participantType: string | null;
  ruleSetVersion: string | null;
  createdAt: string;
  updatedAt: string;
  answers: Record<string, string>;
}

export interface SaveAnswerResult {
  assessmentId: string;
  questionKey: string;
  value: string;
  assessmentStatus: AssessmentStatus;
  updatedAt: string;
}

export interface RecommendationResultDto {
  recommendationId: string;
  assessmentId: string;
  productCode: string;
  displayName: string;
  fitScore: number;
  status: "suggested" | "expertReviewRequired" | string;
  requiresExpertReview: boolean;
  reasons: string[];
  ruleSetVersion: string;
  createdAt: string;
  syntheticLeadId: string;
}

export interface ApiErrorEnvelope {
  error: {
    code: string;
    message: string;
    correlationId?: string;
    fields?: Array<{
      field?: string;
      path?: string;
      code?: string;
      message?: string;
    }>;
  };
}

export type RoiScenarioName = "conservative" | "reference" | "growth";

export interface CalculateRoiRequest {
  scenarioName: RoiScenarioName;
  casesPerMonth: number;
  minutesSavedPerCase: number;
  loadedHourlyCost: number;
  annualOperatingCost: number;
  implementationCost: number;
}

export interface RoiScenarioDto {
  id: string;
  assessmentId: string;
  scenarioName: RoiScenarioName;
  casesPerMonth: number;
  minutesSavedPerCase: number;
  loadedHourlyCost: number;
  annualOperatingCost: number;
  implementationCost: number;
  casesPerYear: number;
  annualHoursSaved: number;
  annualTimeValue: number;
  annualNetBenefit: number;
  simplePaybackMonths: number | null;
  createdAt: string;
  updatedAt: string;
}
