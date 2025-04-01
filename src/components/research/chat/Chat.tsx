import { useEffect, useState, useRef } from "react";
import { Avatar as UserAvatar } from "@heroui/react";
import Avatar from "boring-avatars";
import PromptForm from "./prompt-form";
import {
  Navigate,
  redirect,
  useParams,
  useRouter,
} from "@tanstack/react-router";
import { useSearch } from "@tanstack/react-router";
import * as signalR from "@microsoft/signalr";
import { config } from "../../../../config";
import { store } from "../../../lib/state/store";
import { LogoIcon } from "../../Icons";
import ReactMarkdown from "react-markdown";
import Thinking from "./Thinking";

type Message = {
  id: number;
  text: string;
  sender: "assistant" | "user";
};

interface ChatSearchParams {
  initialQuery?: string;
}

export default function ResearchChat() {
  const router = useRouter();
  const { id } = useParams({ from: "/__app/research/chat/$id/" });
  const searchParams = useSearch({
    from: "/__app/research/chat/$id/",
  }) as ChatSearchParams;
  const initialQuery = searchParams.initialQuery;
  const user = store.state.authState.user;
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

    newConnection.on("ResearchSaved", (id: string) => {
      router.navigate({ to: `/library/${id}` });
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
    <div className="flex flex-col h-full max-w-3xl mx-auto">
      <div className="flex-1 p-4 space-y-9">
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
                    <LogoIcon className="w-10 h-10 min-w-[40px] rounded-full" />
                  ) : user.avatarUrl ? (
                    <UserAvatar
                      src={user.avatarUrl}
                      size="sm"
                      className="mt-1 w-10 h-10 min-w-[40px]"
                    />
                  ) : (
                    <Avatar
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
                        : "text-base max-w-[90%] break-words prose prose-sm dark:prose-invert text-foreground-500 prose-li:text-foreground-500 prose-p:text-foreground-500 prose-headings:text-foreground-500 prose-strong:text-foreground prose-code:text-foreground prose-pre:bg-content2 prose-pre:text-foreground prose-pre:border prose-pre:border-content4 prose-pre:rounded-lg prose-a:text-primary prose-p:leading-relaxed prose-li:leading-relaxed [&>*]:leading-relaxed [&>ul]:list-none [&>ul>li]:relative [&>ul>li]:pl-6 [&>ul>li]:before:absolute [&>ul>li]:before:left-0 [&>ul>li]:before:content-['•'] [&>ul>li]:before:text-primary-400 prose-hr:border-content4 prose-hr:my-5"
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
                  <LogoIcon className="w-10 h-10 min-w-[40px] rounded-full" />
                </div>
                <Thinking />
              </div>
            )}
            <div ref={messagesEndRef} />
          </>
        )}
      </div>
      <div className="sticky bottom-[0px] w-full px-6 md:px-12 pt-5 pb-8 bg-content1">
        <PromptForm onSendMessage={sendMessage} />
      </div>
    </div>
  );
}
