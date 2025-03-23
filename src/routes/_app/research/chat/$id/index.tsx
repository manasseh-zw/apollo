import { createFileRoute } from "@tanstack/react-router";
import ResearchChat from "../../../../../components/research/chat/Chat";
import { protectedLoader } from "../../../../../lib/utils/loaders";

export const Route = createFileRoute("/_app/research/chat/$id/")({
  component: RouteComponent,
  loader: protectedLoader,
});

function RouteComponent() {
  return (
    <div className="w-full h-full overflow-auto">
      <ResearchChat />
    </div>
  );
}
