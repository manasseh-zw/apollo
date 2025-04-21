export type Project = {
  id: number;
  title: string;
  image: string;
  status: string;
  description: string;
  startDate: string;
  expectedEnd: string;
  progress: number;
  endDate: string;
};

export type ResearchProjectResponse = {
  id: string;
  title: string;
  description: string;
  createdAt: string; // ISO date string
  updatedAt: string; // ISO date string
};

export type TimelineItem = {
  id: string;
  text: string;
  active?: boolean;
  timestamp?: string;
  status: 'completed' | 'in_progress' | 'pending';
};

export type SearchResultItem = {
  id: string;
  icon: 'W' | 'NG' | 'LS' | 'H' | string;
  title: string;
  url: string;
  snippet?: string;
  timestamp?: string;
  isNatGeo?: boolean;
  isLiveScience?: boolean;
  isHistory?: boolean;
};

export type ResearchProgress = {
  duration: string;
  sourcesCount: number;
  webPagesCount: number;
  status: 'completed' | 'in_progress' | 'error';
};