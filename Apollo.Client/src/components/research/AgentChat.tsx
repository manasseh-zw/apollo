import { useState, useEffect, useRef } from "react";
import { Avatar } from "@heroui/react";
import { HubConnection } from "@microsoft/signalr";
import type { AgentChatMessage } from "../../lib/types/research";
import { LogoLight } from "../Icons";
import ReactMarkdown from "react-markdown";

interface AgentChatProps {
  connection: HubConnection | null;
  researchId: string;
}

export default function AgentChat({ connection, researchId }: AgentChatProps) {
  const [messages, setMessages] = useState<AgentChatMessage[]>([]);
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
      <div className="flex-1 p-4 space-y-9 font-geist overflow-y-auto">
        {messages.map((message, index) => (
          <div
            key={`${message.timestamp}-${index}`}
            className="flex items-start gap-3"
          >
            <div className="flex-shrink-0">
              <div className="bg-primary p-[.55rem] rounded-full">
                <LogoLight width={22} height={22} />
              </div>
            </div>
            <div className="flex flex-col flex-1 overflow-hidden">
              <div className="text-sm text-gray-500 mb-1">{message.author}</div>
              <div className="prose prose-sm dark:prose-invert">
                <ReactMarkdown>{message.message}</ReactMarkdown>
              </div>
            </div>
          </div>
        ))}
        <div ref={messagesEndRef} />
      </div>
    </div>
  );
}
