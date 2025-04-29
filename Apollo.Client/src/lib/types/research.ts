export enum ResearchStatus {
  InProgress,
  Complete,
}

export enum ResearchType {
  Casual,
  Academic,
  Technical,
}

export enum ResearchDepth {
  Brief,
  Standard,
  Comprehensive,
}

export enum TimelineItemType {
  Question,
  Analysis,
  Synthesis,
}

export enum TimelineItemStatus {
  Pending,
  InProgress,
  Completed,
}

export enum ResearchFeedUpdateType {
  Message,
  Searching,
  SearchResults,
  Snippet,
  TableOfContents,
}

export type ResearchPlan = {
  id: string;
  questions: string[];
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
export type TimelineItem = {
  id: string;
  text: string;
  type: TimelineItemType;
  active: boolean;
  status: TimelineItemStatus;
};

export type TimelineUpdate = {
  researchId: string;
  timestamp: string;
  items: TimelineItem[];
};

// Research Feed Types
export type ResearchFeedUpdateBase = {
  researchId: string;
  timestamp: string;
  type: ResearchFeedUpdateType;
};

export type ProgressMessageUpdate = ResearchFeedUpdateBase & {
  type: ResearchFeedUpdateType.Message;
  message: string;
};

export type WebSearchUpdate = ResearchFeedUpdateBase & {
  type: ResearchFeedUpdateType.Searching;
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
  type: ResearchFeedUpdateType.SearchResults;
  results: SearchResultItem[];
};

export type SnippetUpdate = ResearchFeedUpdateBase & {
  type: ResearchFeedUpdateType.Snippet;
  content: string;
  highlights?: string[];
};

export type TableOfContentsUpdate = ResearchFeedUpdateBase & {
  type: ResearchFeedUpdateType.TableOfContents;
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
