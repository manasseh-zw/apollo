import { createFileRoute } from "@tanstack/react-router";
import ChatLanding from "../../../components/research/chat-landing/ChatLanding";
import { protectedLoader } from "../../../lib/utils/loaders";

export const Route = createFileRoute("/__app/research/")({
  component: Research,
  loader: protectedLoader,
});

function Research() {
  return (
    <div className="bg-content1 w-full h-full flex items-center justify-center p-6 md:px-12 lg:px-16">
      <ChatLanding />
    </div>
  );
}
