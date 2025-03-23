import { useEffect, useState, useRef } from "react";
import { Avatar as UserAvatar } from "@heroui/react";
import Avatar from "boring-avatars";
import Markdown from "markdown-to-jsx";
import PromptForm from "./prompt-form";
import { useParams } from "@tanstack/react-router";
import { useSearch } from "@tanstack/react-router";
import * as signalR from "@microsoft/signalr";
import { config } from "../../../../config";
import { store } from "../../../lib/state/store";

type Message = {
  id: number;
  text: string;
  sender: "assistant" | "user";
};

interface ChatSearchParams {
  initialQuery?: string;
}

const MarkdownOptions = {
  overrides: {
    h1: {
      component: ({ children, ...props }) => (
        <h1 className="text-2xl font-bold mb-3" {...props}>
          {children}
        </h1>
      ),
    },
    h2: {
      component: ({ children, ...props }) => (
        <h2 className="text-xl font-semibold mb-3" {...props}>
          {children}
        </h2>
      ),
    },
    h3: {
      component: ({ children, ...props }) => (
        <h3 className="text-lg font-medium mb-2" {...props}>
          {children}
        </h3>
      ),
    },
    p: {
      component: ({ children, ...props }) => (
        <p className="mb-4 leading-relaxed prose" {...props}>
          {children}
        </p>
      ),
    },
    ul: {
      component: ({ children, ...props }) => (
        <ul className="list-disc ml-6 mb-4 space-y-2" {...props}>
          {children}
        </ul>
      ),
    },
    ol: {
      component: ({ children, ...props }) => (
        <ol
          className="list-decimal ml-6 mb-4 space-y-2 text-gray-700"
          {...props}
        >
          {children}
        </ol>
      ),
    },
    li: {
      component: ({ children, ...props }) => (
        <li className="leading-relaxed" {...props}>
          {children}
        </li>
      ),
    },
    code: {
      component: ({ children, ...props }) => (
        <code
          className="bg-gray-100 dark:bg-gray-800 rounded px-1 py-0.5 font-mono text-sm"
          {...props}
        >
          {children}
        </code>
      ),
    },
    pre: {
      component: ({ children, ...props }) => (
        <pre
          className="bg-gray-100 dark:bg-gray-800 rounded p-4 mb-4 overflow-x-auto font-mono text-sm"
          {...props}
        >
          {children}
        </pre>
      ),
    },
    a: {
      component: ({ children, ...props }) => (
        <a className="text-blue-500 hover:text-blue-600 underline" {...props}>
          {children}
        </a>
      ),
    },
    blockquote: {
      component: ({ children, ...props }) => (
        <blockquote
          className="border-l-4 border-gray-300 pl-4 italic mb-4"
          {...props}
        >
          {children}
        </blockquote>
      ),
    },
  },
};

export default function ResearchChat() {
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
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${config.url}/rtc/research`)
      .withAutomaticReconnect()
      .build();

    newConnection.on("ReceiveResponse", (response: string) => {
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

    try {
      await connection?.invoke("ReceiveMessage", message);
    } catch (error) {
      console.error("Error sending message:", error);
    }
  };

  return (
    <div className="flex flex-col h-full max-w-3xl mx-auto">
      <div className="flex-1 p-4 space-y-6">
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
                    <UserAvatar
                      src="/doodle/brainworm.webp"
                      size="sm"
                      className="mt-1 w-10 h-10 min-w-[40px]"
                    />
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
                        : "max-w-[85%] break-words"
                    }
                  >
                    {message.sender === "assistant" ? (
                      <Markdown className="prose" options={MarkdownOptions}>
                        {message.text}
                      </Markdown>
                    ) : (
                      message.text
                    )}
                  </div>
                </div>
              </div>
            ))}
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
