import type {
  ApiErrorEnvelope,
  AssessmentDto,
  CalculateRoiRequest,
  RecommendationResultDto,
  RoiScenarioDto,
  SaveAnswerResult,
} from "@/lib/api/types";

const DEFAULT_API_BASE_URL = "http://localhost:5158";

export class CandidateApiError extends Error {
  readonly status: number;
  readonly code: string;
  readonly correlationId?: string;

  constructor(params: {
    status: number;
    code: string;
    message: string;
    correlationId?: string;
  }) {
    super(params.message);
    this.name = "CandidateApiError";
    this.status = params.status;
    this.code = params.code;
    this.correlationId = params.correlationId;
  }
}

function getApiBaseUrl(): string {
  return (process.env.NEXT_PUBLIC_API_BASE_URL ?? DEFAULT_API_BASE_URL).replace(/\/$/, "");
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${getApiBaseUrl()}${path}`, {
    ...init,
    headers: {
      Accept: "application/json",
      ...(init?.body ? { "Content-Type": "application/json" } : {}),
      ...init?.headers,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    let envelope: ApiErrorEnvelope | null = null;

    try {
      envelope = (await response.json()) as ApiErrorEnvelope;
    } catch {
      // Keep a safe generic message when the API returns no JSON body.
    }

    throw new CandidateApiError({
      status: response.status,
      code: envelope?.error?.code ?? "REQUEST_FAILED",
      message: envelope?.error?.message ?? `Request failed with status ${response.status}.`,
      correlationId:
        envelope?.error?.correlationId ?? response.headers.get("X-Correlation-ID") ?? undefined,
    });
  }

  return (await response.json()) as T;
}

export const candidateApi = {
  createAssessment(): Promise<AssessmentDto> {
    return request<AssessmentDto>("/api/v1/assessments", {
      method: "POST",
    });
  },

  getAssessment(assessmentId: string): Promise<AssessmentDto> {
    return request<AssessmentDto>(`/api/v1/assessments/${assessmentId}`);
  },

  saveAnswer(assessmentId: string, questionKey: string, value: string): Promise<SaveAnswerResult> {
    return request<SaveAnswerResult>(
      `/api/v1/assessments/${assessmentId}/answers/${encodeURIComponent(questionKey)}`,
      {
        method: "PUT",
        body: JSON.stringify({ value }),
      },
    );
  },

  evaluateAssessment(assessmentId: string): Promise<RecommendationResultDto> {
    return request<RecommendationResultDto>(`/api/v1/assessments/${assessmentId}/evaluate`, {
      method: "POST",
    });
  },

  getResult(assessmentId: string): Promise<RecommendationResultDto> {
    return request<RecommendationResultDto>(`/api/v1/assessments/${assessmentId}/result`);
  },

  calculateRoi(assessmentId: string, payload: CalculateRoiRequest): Promise<RoiScenarioDto> {
    return request<RoiScenarioDto>(`/api/v1/assessments/${assessmentId}/roi`, {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },
};
