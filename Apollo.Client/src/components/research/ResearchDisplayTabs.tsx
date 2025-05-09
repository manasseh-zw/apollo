import { Tabs, Tab } from "@heroui/react";
import type { ResearchResponse } from "../../lib/types/research";
import ResearchReport from "./report/ResearchReport";
import ResearchMindMapFlow from "./mindmap/ResearchMindMapFlow";

interface ResearchDisplayTabsProps {
  research: ResearchResponse;
}

export default function ResearchDisplayTabs({
  research,
}: ResearchDisplayTabsProps) {
  return (
    <div className="flex flex-col h-full bg-white">
      <Tabs aria-label="Research Content" className="mx-auto mt-5">
        <Tab key="report" title="Report" className="flex-1">
          <div className="pt-4">
            <ResearchReport report={research.report} />
          </div>
        </Tab>
        <Tab key="mindmap" title="Mind Map" className="flex-1">
          <div className="pt-4">
            {research.mindMap?.graphData ? (
              <ResearchMindMapFlow mindMapData={research.mindMap.graphData} />
            ) : (
              <div className="flex items-center justify-center h-[500px] text-gray-500">
                Mind map data not available
              </div>
            )}
          </div>
        </Tab>
      </Tabs>
    </div>
  );
}
