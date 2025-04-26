// Research Entity Types (matching backend)
export enum ResearchStatus {
  InProgress = "InProgress",
  Complete = "Complete",
}

export enum ResearchType {
  Casual = "Casual",
  Academic = "Academic",
  Technical = "Technical",
}

export enum ResearchDepth {
  Brief = "Brief",
  Standard = "Standard",
  Comprehensive = "Comprehensive",
}

export type ResearchPlan = {
  id: string;
  questions: string[];
  type: ResearchType;
  depth: ResearchDepth;
};

export type ResearchReport = {
  id: string;
  content: string;
};

export type ResearchResponse = {
  id: string;
  title: string;
  description: string;
  plan: ResearchPlan;
  report: ResearchReport | null;
  startedAt: string;
  status: ResearchStatus;
};

// Timeline Types
export type QuestionStatus = "pending" | "in_progress" | "completed";

export type QuestionTimelineItem = {
  id: string;
  text: string;
  active: boolean;
  status: QuestionStatus;
};

export type QuestionTimelineUpdate = {
  researchId: string;
  timestamp: string;
  questions: QuestionTimelineItem[];
};

// Research Feed Types
export type ResearchFeedUpdateBase = {
  researchId: string;
  timestamp: string;
  type: string;
};

export type ProgressMessageUpdate = ResearchFeedUpdateBase & {
  type: "message";
  message: string;
};

export type WebSearchUpdate = ResearchFeedUpdateBase & {
  type: "searching";
  query: string;
};

export type SearchResultItem = {
  id: string;
  icon: string;
  title: string;
  url: string;
  snippet?: string;
  highlights?: string[];
};

export type SearchResultsUpdate = ResearchFeedUpdateBase & {
  type: "search_results";
  results: SearchResultItem[];
};

export type SnippetUpdate = ResearchFeedUpdateBase & {
  type: "snippet";
  content: string;
  highlights?: string[];
};

export type TableOfContentsUpdate = ResearchFeedUpdateBase & {
  type: "toc";
  sections: string[];
};

export type ResearchFeedUpdate =
  | ProgressMessageUpdate
  | WebSearchUpdate
  | SearchResultsUpdate
  | SnippetUpdate
  | TableOfContentsUpdate;

// Agent Chat Types
export type AgentChatMessage = {
  researchId: string;
  timestamp: string;
  author: string;
  message: string;
};

// API Response Types
export type ResearchHistoryItem = {
  id: string;
  title: string;
  startedAt: string;
};

export type PaginatedResponse<T> = {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  hasMore: boolean;
};
