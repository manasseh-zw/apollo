import { useState, lazy, Suspense } from "react";
import { Logo } from "../Icons";
import ChatLandingPromptForm from "./chat-landing/ChatLandingPromptForm";

// Lazy load the chat conversation component
const ChatConversation = lazy(() => import("./chat/Chat"));

export default function ResearchChat() {
  const [isConversationStarted, setIsConversationStarted] = useState(false);
  const [sessionId, setSessionId] = useState<string | null>(null);
  const [initialQuery, setInitialQuery] = useState<string | null>(null);

  // This will be called from the ChatLandingPromptForm
  const handleStartConversation = (id: string, query: string) => {
    setSessionId(id);
    setInitialQuery(query);
    setIsConversationStarted(true);
  };

  if (isConversationStarted && sessionId && initialQuery) {
    return (
      <Suspense
        fallback={
          <div className="w-full h-full flex items-center justify-center">
            Loading conversation...
          </div>
        }
      >
        <ChatConversation id={sessionId} initialQuery={initialQuery} />
      </Suspense>
    );
  }

  return (
    <div className="flex flex-col items-center w-full max-w-xl mx-auto p-4 mb-10 space-y-9">
      <div className="flex flex-col justify-center items-center gap-5">
        <Logo height={38} width={38} />
        <div className="flex flex-col text-center text-black tracking-tight gap-1">
          <h1 className="text-xl md:text-3xl">Hey! I'm Apollo</h1>
          <small className="font-light text-medium">
            Ask. Explore. Understand.
          </small>
        </div>
      </div>

      <div className="w-full">
        <ChatLandingPromptForm onStartConversation={handleStartConversation} />
      </div>
    </div>
  );
}
