"use client";

import React from "react";
import { Button, Chip, cn, Tooltip } from "@heroui/react";
import { ArrowUp } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";
import ChatLandingPromptInput from "./ChatLandingPromptInput";
import { OpenAI } from "../../Icons";

export default function ChatLandingPromptForm() {
  const [prompt, setPrompt] = React.useState("");
  const navigate = useNavigate();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmedPrompt = prompt.trim();
    if (trimmedPrompt) {
      const chatId = crypto.randomUUID();
      navigate({
        to: "/research/chat/$id",
        params: { id: chatId },
        search: { initialQuery: trimmedPrompt },
      });
    }
  };

  return (
    <div className="flex flex-col w-full gap-4">
      <form
        className="flex w-full flex-col items-start bg-white rounded-3xl shadow-[0_2px_4px_0_rgba(0,0,0,0.08)] border border-content2 overflow-hidden"
        onSubmit={handleSubmit}
      >
        <div className="w-full">
          <ChatLandingPromptInput
            value={prompt}
            onValueChange={setPrompt}
            initialRows={2}
            maxHeightRem={10}
            placeholder="Ask me anything ..."
          />
        </div>

        <div className="flex w-full items-center justify-between px-3 pb-2 bg-white">
          <div className="flex items-center gap-2">
            <Chip
              startContent={<OpenAI width={14} height={14} />}
              classNames={{
                base: "font-light cursor-pointer",
              }}
              variant="light"
              size="sm"
            >
              Gpt 4
            </Chip>
          </div>
          <Tooltip showArrow content="Start Research">
            <Button
              isIconOnly
              color={!prompt.trim() ? "default" : "primary"}
              isDisabled={!prompt.trim()}
              radius="full"
              size="sm"
              variant={!prompt.trim() ? "flat" : "solid"}
              type="submit"
              className="bg-black text-white"
            >
              <ArrowUp
                className={cn(
                  "[&>path]:stroke-[2px]",
                  !prompt.trim() ? "text-default-600" : "text-white"
                )}
                width={18}
              />
            </Button>
          </Tooltip>
        </div>
      </form>
    </div>
  );
}
