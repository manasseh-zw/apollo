import { createFileRoute } from "@tanstack/react-router";
import AgentChat from "../../../../components/library/AgentChat";

export const Route = createFileRoute("/__app/library/$id/")({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <main className="w-full h-full grid grid-cols-12 gap-4 p-4">
      <div className="h-full col-span-8 bg-content1 rounded-large p-4 flex flex-col overflow-hidden">
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-medium font-medium text-primary">Current Research</h2>
          <div className="text-tiny text-primary-400">Analyzing quantum computing impact...</div>
        </div>
        <div className="flex-1 bg-content2 rounded-large p-4 overflow-auto">
          <img src="/demo/manus.png" className="aspect-video w-full rounded-large mb-4 object-cover" />
          <div className="prose prose-sm dark:prose-invert max-w-none text-primary">
            <h3>Key Findings</h3>
            <ul>
              <li>Recent breakthroughs in quantum algorithms</li>
              <li>Post-quantum encryption standards development</li>
              <li>Implementation challenges in current systems</li>
              <li>Timeline predictions for quantum supremacy</li>
            </ul>
          </div>
        </div>
      </div>
      <div className="h-full col-span-4 overflow-hidden">
        <AgentChat />
      </div>
    </main>
  );
}
