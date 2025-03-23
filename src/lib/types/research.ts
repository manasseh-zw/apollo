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
  