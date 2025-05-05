import { createContext, useContext, useState } from "react";

interface ResearchContextType {
  refreshHistory: () => void;
  lastRefreshTimestamp: number;
}

const ResearchContext = createContext<ResearchContextType | null>(null);

export function useResearch() {
  const context = useContext(ResearchContext);
  if (!context) {
    throw new Error("useResearch must be used within a ResearchProvider");
  }
  return context;
}

export function ResearchProvider({ children }: { children: React.ReactNode }) {
  const [lastRefreshTimestamp, setLastRefreshTimestamp] = useState(Date.now());

  const refreshHistory = () => {
    setLastRefreshTimestamp(Date.now());
  };

  return (
    <ResearchContext.Provider value={{ refreshHistory, lastRefreshTimestamp }}>
      {children}
    </ResearchContext.Provider>
  );
}
