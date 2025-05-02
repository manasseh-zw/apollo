import type { ComponentProps } from "react";
import { cn, Spinner } from "@heroui/react";
import React from "react";
import { m, LazyMotion, domAnimation } from "framer-motion";
import { Check, FileSearch, FileText } from "lucide-react";
import {
  TimelineItemStatus,
  TimelineItemType,
  type TimelineItem,
} from "../../../lib/types/research";

// --- Props for the simplified VerticalTimeline component ---
export interface VerticalTimelineProps
  extends React.HTMLAttributes<HTMLDivElement> {
  items?: TimelineItem[];
  hideProgressBars?: boolean;
  className?: string;
  itemClassName?: string;
}

function AnimatedCheckIcon(props: ComponentProps<"svg">) {
  return (
    <svg
      {...props}
      fill="none"
      stroke="currentColor"
      strokeWidth={2.5}
      viewBox="0 0 24 24"
    >
      <m.path
        key="check"
        animate={{ pathLength: 1 }}
        d="M5 13l4 4L19 7"
        initial={{ pathLength: 0 }}
        strokeLinecap="round"
        strokeLinejoin="round"
        transition={{
          delay: 0.2,
          type: "tween",
          ease: "easeOut",
          duration: 0.3,
        }}
      />
    </svg>
  );
}

// --- The Simplified VerticalTimeline Component ---
const VerticalTimeline = React.forwardRef<
  HTMLDivElement,
  VerticalTimelineProps
>(
  (
    {
      items = [],
      hideProgressBars = false,
      itemClassName,
      className,
      ...props
    },
    ref
  ) => {
    const getTimelineIcon = (item: TimelineItem): React.ReactNode => {
      const status = getStatusString(item.status);

      // Completed: Always show AnimatedCheckIcon with primary foreground color
      if (status === "complete") {
        return (
          <AnimatedCheckIcon className="h-5 w-5 text-primary-foreground" />
        );
      }

      // In Progress: Always show Spinner with primary color
      if (status === "active") {
        return <Spinner size="sm" />;
      }

      // Pending: Show icon based on type with default muted color
      if (status === "inactive") {
        // Using text-default-500 (#6c757d) for pending icons
        const iconProps = { className: "h-4 w-4 text-default-500" };
        switch (item.type) {
          case TimelineItemType.Question:
            return <Check {...iconProps} />;
          case TimelineItemType.Analysis:
            return <FileSearch {...iconProps} />;
          case TimelineItemType.Synthesis:
            return <FileText {...iconProps} />;
          default:
            // Default fallback icon for pending items (simple dot)
            return <div className="h-2 w-2 rounded-full bg-default-500" />;
        }
      }

      // Fallback for any unexpected status
      return <div className="h-2 w-2 rounded-full bg-gray-400" />;
    };

    // --- Map TimelineItemStatus to internal status string ---
    const getStatusString = (
      status: TimelineItemStatus
    ): "inactive" | "active" | "complete" => {
      switch (status) {
        case TimelineItemStatus.Completed:
          return "complete";
        case TimelineItemStatus.InProgress:
          return "active";
        case TimelineItemStatus.Pending:
          return "inactive";
        default:
          return "inactive";
      }
    };

    return (
      // Use a div, apply custom className
      <div ref={ref} className={cn("flex flex-col", className)} {...props}>
        {items?.map((item, itemIdx) => {
          const status = getStatusString(item.status);
          const isLastItem = itemIdx === items.length - 1;

          return (
            // Relative container for each item + its connecting line
            <div
              key={item.id}
              className={cn("relative flex items-start pb-6", itemClassName)}
            >
              {/* Progress line (conditionally rendered) */}
              {!hideProgressBars && !isLastItem && (
                <div
                  aria-hidden="true"
                  className={cn(
                    "pointer-events-none absolute left-[14px] top-[28px] bottom-[-1px] w-0.5 bg-default-200",
                    // Active part of the line (overlay) - uses primary color (#252525)
                    "after:absolute after:block after:top-0 after:left-0 after:h-0 after:w-full after:bg-primary/90 after:transition-[height] after:duration-300 after:content-['']",
                    {
                      // Fill the line below if the *current* item is complete
                      "after:h-full": status === "complete",
                    }
                  )}
                />
              )}

              {/* Icon and Text Content Wrapper */}
              <div className="flex w-full items-start gap-4">
                {/* Icon Container - z-10 ensures it's above the line */}
                <div className="flex h-full items-center z-10">
                  <LazyMotion features={domAnimation}>
                    {/* Applying styles directly using cn based on status */}
                    <m.div
                      animate={status} // Still useful for triggering animation on status change
                      className={cn(
                        "relative flex h-[28px] w-[28px] shrink-0 items-center justify-center rounded-full border-2", // Standard size & border
                        // --- Status-specific styling using Tailwind classes ---
                        {
                          // Inactive/Pending: default-300 border (#ced4da), page background
                          "border-default-300 bg-background":
                            status === "inactive",
                          // Active/In Progress: primary border (#252525), page background
                          "border-primary bg-background border-0":
                            status === "active",
                          // Completed: primary border & background (#252525), shadow
                          "border-primary bg-primary/95 shadow-md":
                            status === "complete",
                        }
                        // --- End Status-specific styling ---
                      )}
                      data-status={status}
                      initial={false}
                      transition={{ duration: 0.25 }} // Keep transition for smoothness
                    >
                      {/* Render the correct icon */}
                      {getTimelineIcon(item)}
                    </m.div>
                  </LazyMotion>
                </div>

                {/* Text Content */}
                <div className="flex-1 text-left">
                  <div
                    className={cn(
                      "text-sm transition-[color] duration-300", // Base text style
                      // --- Status-specific text styling ---
                      {
                        // Completed: Use default foreground (dark text #212529 on light bg)
                        "font-medium text-default-foreground":
                          status === "complete",
                        // Active: Use primary color text (#252525)
                        "font-medium text-primary": status === "active",
                        // Pending: Use default-500 text (#6c757d)
                        "text-default-500": status === "inactive",
                      }
                      // --- End Status-specific text styling ---
                    )}
                  >
                    {item.text}
                  </div>
                </div>
              </div>
            </div>
          );
        })}
      </div>
    );
  }
);

VerticalTimeline.displayName = "VerticalTimeline";

export default VerticalTimeline;
