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
          inputWrapper: " border-1 border-secondary-100 shadow-sm bg-content2",
          label: cn("hidden", classNames?.label),
          input: cn("py-0", classNames?.input),
        }}
        minRows={1}
        placeholder="Enter your reply here..."
        radius="lg"
        variant="bordered"
        {...props}
      />
    );
  }
);

export default PromptInput;

PromptInput.displayName = "PromptInput";
