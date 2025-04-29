import { apiRequest, type ApiResponse } from "../utils/api";
import type {
  ResearchResponse,
  ResearchHistoryItem,
  PaginatedResponse,
} from "../types/research";

export async function getResearchById(
  id: string
): Promise<ApiResponse<ResearchResponse>> {
  return apiRequest<ResearchResponse>(`/api/research/${id}`);
}

export async function getResearchHistory(
  page: number = 1,
  pageSize: number = 5
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
