import { useState, useEffect, useRef } from "react";
import { Avatar, AvatarGroup } from "@heroui/react";
import { HubConnection } from "@microsoft/signalr";
import type { AgentChatMessage } from "../../lib/types/research";
import ReactMarkdown from "react-markdown";

interface AgentChatProps {
  connection: HubConnection | null;
  researchId: string;
  initialChatMessages?: AgentChatMessage[];
}

const agentPersonas: Record<string, { name: string; avatar: string }> = {
  "ResearchCoordinator": { name: "Apollo", avatar: "/agents/agent1.jpg" },
  "ResearchEngine": { name: "Athena", avatar: "/agents/agent2.jpg" },
  "ResearchAnalyzer": { name: "Hermes", avatar: "/agents/agent3.jpg" },
};

const activeAgents = Object.values(agentPersonas);

export default function AgentChat({
  connection,
  researchId,
  initialChatMessages,
}: AgentChatProps) {
  const [messages, setMessages] = useState<AgentChatMessage[]>(
    initialChatMessages ?? []
  );
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  useEffect(() => {
    if (!connection) return;

    connection.on("ReceiveAgentChatMessage", (message: AgentChatMessage) => {
      setMessages((prev) => [...prev, message]);
    });

    return () => {
      connection.off("ReceiveAgentChatMessage");
    };
  }, [connection]);

  return (
    <div className="flex flex-col h-full w-full">
      <div className="sticky top-0 bg-white border-b border-gray-200 py-3 px-2">
        <div className="flex items-center justify-between">
          <h2 className="text-md  text-primary">Agent Group Chat</h2>
          <AvatarGroup  max={3} size="sm">
            {activeAgents.map((agent) => (
              <Avatar
                key={agent.name}
                src={agent.avatar}
                size="sm"
                className="w-8 h-8"
              />
            ))}
          </AvatarGroup>
        </div>
      </div>

      <div className="flex-1 p-4 space-y-6 font-geist overflow-y-auto">
        {messages.map((message, index) => {
          const persona = agentPersonas[message.author] || {
            name: "Unknown",
            avatar: "/agents/agent1.jpg",
          };

          return (
            <div
              key={`${message.timestamp}-${index}`}
              className="flex items-start gap-3"
            >
              <Avatar
                src={persona.avatar}
                size="sm"
                className="w-8 h-8 rounded-full flex-shrink-0"
              />
              <div className="flex flex-col flex-1 overflow-hidden">
                <div className="flex items-center gap-2">
                  <span className="text-sm font-medium text-gray-700">
                    {persona.name}
                  </span>
                  <span className="text-xs text-gray-500">
                    {new Date(message.timestamp).toLocaleTimeString()}
                  </span>
                </div>
                <div className="mt-1 prose prose-sm dark:prose-invert">
                  <ReactMarkdown>{message.message}</ReactMarkdown>
                </div>
              </div>
            </div>
          );
        })}
        <div ref={messagesEndRef} />
      </div>
    </div>
  );
}
