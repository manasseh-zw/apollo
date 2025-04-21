import { createFileRoute } from "@tanstack/react-router";
import AgentChat from "../../../../components/research/AgentChat";
import { Image } from "@heroui/react";

export const Route = createFileRoute("/__app/research/$id/")({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <main className="w-full h-full grid grid-cols-12 gap-4 p-4 ">
      <div className="h-full col-span-8  p-4 flex flex-col overflow-hidden border-r-1 border-secondary-200 ">
        <div className="mb-4 flex items-center justify-between  ">
          <h2 className="text-medium font-medium text-primary">
            Quantum Computing
          </h2>
          <div className="text-tiny text-primary-600">
            Analyzing quantum computing impact...
          </div>
        </div>
        <div className="flex-1  p-4 overflow-auto rounded-sm">
          <Image
            src="/demo/manus.png"
            className="aspect-video w-full  mb-4 object-cover rounded-md"
          />
          <div className="prose prose-sm dark:prose-invert max-w-none text-primary">
            <h4 className="text-primary">Key Findings</h4>
            <ul>
              <li>Recent breakthroughs in quantum algorithms</li>
              <li>Post-quantum encryption standards development</li>
              <li>Implementation challenges in current systems</li>
              <li>Timeline predictions for quantum supremacy</li>
            </ul>
          </div>
        </div>
      </div>
      <div className="h-full col-span-4 overflow-hidden  ">
        <AgentChat />
      </div>
    </main>
  );
}
