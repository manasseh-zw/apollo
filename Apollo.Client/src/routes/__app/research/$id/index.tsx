import { createFileRoute } from "@tanstack/react-router";
import AgentChat from "../../../../components/research/AgentChat";
import ResearchFeed from "../../../../components/research/feed/ResearchFeed";

export const Route = createFileRoute("/__app/research/$id/")({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <main className="w-full h-full grid grid-cols-12 gap-4 bg-white p-1">
      <div className="h-full col-span-8 border-r border-gray-200">
        <ResearchFeed />
      </div>
      <div className="h-full col-span-4 flex flex-col overflow-hidden py-2">
        <AgentChat />
      </div>
    </main>
  );
}
