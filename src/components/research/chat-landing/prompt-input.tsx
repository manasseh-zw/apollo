import type { TextAreaProps } from "@heroui/react";
import React from "react";
import { cn, Textarea } from "@heroui/react";

const PromptInput = React.forwardRef<HTMLTextAreaElement, TextAreaProps>(
  ({ classNames = {}, ...props }, ref) => {
    return (
      <Textarea
        color="primary"
        ref={ref}
        aria-label="Prompt"
        className="min-h-[40px] "
        classNames={{
          ...classNames,
          inputWrapper: "border-1 border-secondary-400 shadow-sm bg-content1 ",
          label: cn("hidden", classNames?.label),
          input: cn("py-0 placeholder:text-secondary-600 font-light", classNames?.input),
        }}
        minRows={1}
        placeholder="Research Anything..."
        radius="lg"
        variant="bordered"
        {...props}
      />
    );
  }
);

export default PromptInput;

PromptInput.displayName = "PromptInput";
