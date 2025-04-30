import type React from "react";
import { forwardRef, useRef, useEffect, useImperativeHandle } from "react";
import { cn } from "@heroui/react";
import { motion, type Transition } from "framer-motion";

export interface ChatLandingPromptInputProps {
	value: string;
	onValueChange: (value: string) => void;
	className?: string;
	placeholder?: string;
	initialRows?: number;
	maxHeightRem?: number;
	[key: string]: any;
}

export interface ChatLandingPromptInputRef {
	textarea: HTMLTextAreaElement | null;
}

const ChatLandingPromptInput = forwardRef<
	ChatLandingPromptInputRef,
	ChatLandingPromptInputProps
>(
	(
		{
			value,
			onValueChange,
			className,
			placeholder = "Ask anything...",
			initialRows = 2,
			maxHeightRem = 10,
			...props
		},
		ref,
	) => {
		const textareaRef = useRef<HTMLTextAreaElement>(null);

		useImperativeHandle(ref, () => ({
			textarea: textareaRef.current,
		}));

		useEffect(() => {
			const textarea = textareaRef.current;
			if (textarea) {
				textarea.style.height = "auto";
				const scrollHeight = textarea.scrollHeight;
				const maxHeightPx =
					maxHeightRem *
					Number.parseFloat(
						getComputedStyle(document.documentElement).fontSize,
					);
				textarea.style.height = `${Math.min(scrollHeight, maxHeightPx)}px`;
				textarea.style.overflowY =
					scrollHeight > maxHeightPx ? "scroll" : "hidden";
			}
		}, [value, maxHeightRem]);

		const handleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
			onValueChange(event.target.value);
		};

		const transition: Transition = {
			type: "spring",
			stiffness: 100,
			damping: 30,
			duration: 0.2,
		};

		return (
			<motion.textarea
				ref={textareaRef}
				value={value}
				onChange={handleChange}
				rows={initialRows}
				placeholder={placeholder}
				aria-label="Prompt"
				className={cn(
					"w-full block",
					"bg-transparent",
					"border-0 shadow-none outline-none",
					"p-4",
					"text-sm text-gray-700",
					"placeholder:text-secondary-500 placeholder:font-light",
					"box-border",
					"resize-none",
					`max-h-[${maxHeightRem}rem]`,
					"overflow-y-hidden",
					"scrollbar-hide",
					className,
				)}
				transition={transition}
				{...props}
			/>
		);
	},
);

ChatLandingPromptInput.displayName = "ChatLandingPromptInput";

export default ChatLandingPromptInput;
