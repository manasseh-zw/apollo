import { useEffect, useState, useRef } from "react";
import { Avatar as UserAvatar } from "@heroui/react";
import Avatar from "boring-avatars";
import ChatPromptForm from "./ChatPromptForm";
import * as signalR from "@microsoft/signalr";
import { config } from "../../../../config";
import { store } from "../../../lib/state/store";
import ReactMarkdown from "react-markdown";
import Thinking from "./Thinking";
import { LogoLight } from "../../Icons";
import { useRouter } from "@tanstack/react-router";
import { useResearch } from "../../../contexts/ResearchContext";

type Message = {
  id: number;
  text: string;
  sender: "assistant" | "user";
};

interface ChatProps {
  id: string;
  initialQuery: string;
}

export default function Chat({ id, initialQuery }: ChatProps) {
  const user = store.state.authState.user;
  const router = useRouter();
  const { refreshHistory } = useResearch();
  const [messages, setMessages] = useState<Message[]>([]);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );
  const [isConnected, setIsConnected] = useState(false);
  const [isThinking, setIsThinking] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${config.url}/hubs/research`)
      .withAutomaticReconnect()
      .build();

    newConnection.on("ReceiveResponse", (response: string) => {
      setIsThinking(false);
      setMessages((prev) => {
        const lastMessage = prev[prev.length - 1];

        if (!lastMessage || lastMessage.sender === "user") {
          return [
            ...prev,
            {
              id: prev.length + 1,
              text: response,
              sender: "assistant",
            },
          ];
        }

        return [
          ...prev.slice(0, -1),
          {
            ...lastMessage,
            text: lastMessage.text + response,
          },
        ];
      });
    });

    newConnection.on("ResearchSaved", (researchId: string) => {
      refreshHistory();
      router.navigate({
        to: "/research/$id",
        params: { id: researchId },
      });
    });

    newConnection
      .start()
      .then(() => {
        setConnection(newConnection);
        setIsConnected(true);
        if (id && initialQuery) {
          newConnection.invoke("StartResearchChat", id, initialQuery);
          setMessages([
            {
              id: 1,
              text: initialQuery,
              sender: "user",
            },
          ]);
        }
      })
      .catch((err) => console.error("SignalR Connection Error: ", err));

    return () => {
      newConnection.stop();
    };
  }, [id, initialQuery]);

  const sendMessage = async (message: string) => {
    if (!isConnected || !message.trim()) return;

    setMessages((prev) => [
      ...prev,
      {
        id: prev.length + 1,
        text: message,
        sender: "user",
      },
    ]);

    setIsThinking(true);

    try {
      await connection?.invoke("ReceiveMessage", message);
    } catch (error) {
      console.error("Error sending message:", error);
      setIsThinking(false);
    }
  };

  return (
    <div className="flex flex-col h-full w-full max-w-3xl mx-auto">
      <div className="flex-1 p-4 space-y-9 font-geist">
        {messages.length === 0 ? (
          <div className="flex items-center justify-center h-full">
            <div className="text-center text-gray-500">
              <p>Start your research conversation...</p>
              <p className="text-sm mt-2">Project ID: {id}</p>
            </div>
          </div>
        ) : (
          <>
            {messages.map((message) => (
              <div
                key={message.id}
                className={`flex ${
                  message.sender === "user" ? "flex-row-reverse" : "flex-row"
                } items-start gap-3`}
              >
                <div className="flex-shrink-0">
                  {message.sender === "assistant" ? (
                    <div className="bg-primary p-[.55rem] rounded-full">
                      <LogoLight width={22} height={22} />
                    </div>
                  ) : user.avatarUrl ? (
                    <UserAvatar
                      src={user.avatarUrl}
                      size="sm"
                      className="mt-1 w-10 h-10 min-w-[40px]"
                    />
                  ) : (
                    <Avatar
                      size="sm"
                      name={user.username}
                      className="w-10 h-10 min-w-[40px]"
                    />
                  )}
                </div>
                <div
                  className={`flex flex-col ${
                    message.sender === "user" ? "items-end" : "items-start"
                  } flex-1 overflow-hidden`}
                >
                  <div
                    className={
                      message.sender === "user"
                        ? "rounded-2xl bg-content2 p-3 max-w-[85%] break-words"
                        : "text-base max-w-[90%] break-words prose prose-sm dark:prose-invert text-primary prose-li:text-primary prose-p:text-primary prose-headings:text-primary prose-strong:text-foreground prose-code:text-foreground prose-pre:bg-content2 prose-pre:text-foreground prose-pre:border prose-pre:border-content4 prose-pre:rounded-lg prose-a:text-primary prose-p:leading-relaxed prose-li:leading-relaxed [&>*]:leading-relaxed [&>ul]:list-none [&>ul>li]:relative [&>ul>li]:before:absolute [&>ul>li]:before:left-0 [&>ul>li]:before:content-['•'] [&>ul>li]:before:text-primary-400 prose-hr:border-content4 prose-hr:my-5"
                    }
                  >
                    {message.sender === "assistant" ? (
                      <ReactMarkdown>{message.text}</ReactMarkdown>
                    ) : (
                      message.text
                    )}
                  </div>
                </div>
              </div>
            ))}
            {isThinking && (
              <div className="flex flex-row items-center gap-3">
                <div className="flex-shrink-0">
                  <div className="bg-primary p-[.55rem] rounded-full">
                    <LogoLight width={22} height={22} />
                  </div>
                </div>
                <Thinking />
              </div>
            )}
            <div ref={messagesEndRef} />
          </>
        )}
      </div>
      <div className="sticky bottom-[0px] w-full px-6 md:px-12 pt-5 pb-8 bg-content1">
        <ChatPromptForm onSendMessage={sendMessage} />
      </div>
    </div>
  );
}
