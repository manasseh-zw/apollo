import React from "react";
import { Button, cn, Tooltip } from "@heroui/react";
import PromptInput from "./prompt-input";
import { ArrowUp } from "lucide-react";
import { useNavigate } from "@tanstack/react-router";

interface PromptFormProps {
  onSendMessage?: (message: string) => void;
}

export default function PromptForm({ onSendMessage }: PromptFormProps) {
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
        search: { initialQuery: trimmedPrompt }
      });
    }
  };

  return (
    <form className="flex w-full items-start gap-2" onSubmit={handleSubmit}>
      <PromptInput
        classNames={{
          innerWrapper: "relative w-full",
          input: "pt-1 pb-2 !pr-10 text-medium",
        }}
        endContent={
          <Tooltip showArrow content="Start Research">
            <Button
              isIconOnly
              color={!prompt.trim() ? "default" : "primary"}
              isDisabled={!prompt.trim()}
              radius="lg"
              size="sm"
              variant={!prompt.trim() ? "flat" : "solid"}
              type="submit"
            >
              <ArrowUp
                className={cn(
                  "[&>path]:stroke-[2px]",
                  !prompt.trim()
                    ? "text-default-600"
                    : "text-primary-foreground"
                )}
                width={20}
              />
            </Button>
          </Tooltip>
        }
        minRows={1}
        radius="lg"
        value={prompt}
        onValueChange={setPrompt}
      />
    </form>
  );
}