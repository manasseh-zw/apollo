import { apiRequest, type ApiResponse } from "../utils/api";
import type {
  ResearchResponse,
  ResearchHistoryItem,
  PaginatedResponse,
  ResearchUpdatesResponse,
  SharedResearchReport,
} from "../types/research";

export async function getResearchById(
  id: string
): Promise<ApiResponse<ResearchResponse>> {
  return apiRequest<ResearchResponse>(`/api/research/${id}`);
}

export async function getResearchHistory(
  page = 1,
  pageSize = 5
): Promise<ApiResponse<PaginatedResponse<ResearchHistoryItem>>> {
  return apiRequest<PaginatedResponse<ResearchHistoryItem>>(
    `/api/research/history?page=${page}&pageSize=${pageSize}`
  );
}

export async function getAllResearch(): Promise<
  ApiResponse<ResearchResponse[]>
> {
  return apiRequest<ResearchResponse[]>("/api/research");
}

export type CreateResearchRequest = {
  title: string;
  description: string;
  questions: string[];
};

export type CreateResearchResponse = {
  id: string;
  title: string;
  description: string;
  startedAt: string;
  status: string;
};

export async function createResearch(
  data: CreateResearchRequest
): Promise<ApiResponse<CreateResearchResponse>> {
  return apiRequest<CreateResearchResponse>("/api/research", {
    method: "POST",
    body: JSON.stringify(data),
  });
}

export async function getResearchUpdates(
  id: string
): Promise<ApiResponse<ResearchUpdatesResponse>> {
  return apiRequest<ResearchUpdatesResponse>(`/api/research/${id}/updates`);
}

// New method to fetch a shared research report
export async function getSharedResearchReport(
  reportId: string
): Promise<ApiResponse<SharedResearchReport>> {
  return apiRequest<SharedResearchReport>(`/api/research/share/${reportId}`);
}
