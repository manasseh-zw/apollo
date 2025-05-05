import React from "react";
import { cn } from "@heroui/react";
import { Link } from "@tanstack/react-router";
import { Tooltip, Skeleton, Button } from "@heroui/react";
import { DynamicIcon, type IconName } from "lucide-react/dynamic";
import type { ResearchHistoryItem } from "../../lib/types/research";
import { ChevronDown } from "lucide-react";

export type SidebarItem = {
  key: string;
  title: string;
  iconName: IconName;
  href?: string;
};

export const sidebarItems: SidebarItem[] = [];

export type SidebarProps = {
  isCompact?: boolean;
  className?: string;
  historyItems: ResearchHistoryItem[];
  isLoadingInitial: boolean;
  error: string | null;
  hasMore: boolean;
  handleLoadMore: () => void;
  activeResearchId: string | null;
  showRecentChats: boolean;
  isLoadingMore?: boolean;
};

const SidebarNav = React.forwardRef<HTMLElement, SidebarProps>(
  (
    {
      isCompact,
      className,
      historyItems,
      isLoadingInitial,
      error,
      hasMore,
      handleLoadMore,
      activeResearchId,
      showRecentChats,
      isLoadingMore = false,
    },
    ref
  ) => {
    const renderHistoryItems = () => {
      if (!showRecentChats) return null;

      if (isLoadingInitial) {
        return Array(3)
          .fill(0)
          .map((_, i) => (
            <div key={i} className="px-2 min-h-5 flex items-center">
              <Skeleton className="w-full h-3 rounded-md bg-primary-700/30" />
            </div>
          ));
      }

      if (error) {
        return null;
      }

      return (
        <>
          {historyItems.map((item) => {
            const isActive = activeResearchId === item.id;
            console.log(
              `SidebarNav: Item ${item.title} (${item.id}), Active ID: ${activeResearchId}, IsActive: ${isActive}`
            );

            const historyItemContent = (
              <Link
                to="/research/$id"
                params={{ id: item.id }}
                className={cn(
                  "flex items-center px-3 min-h-11 rounded-large h-[44px] transition-colors relative group",
                  {
                    "w-11 h-10 justify-center p-0": isCompact,
                    "bg-[#333333] text-primary-foreground": isActive,
                    "hover:bg-primary-800/30 text-primary-foreground/80 hover:text-primary-foreground":
                      !isActive,
                  }
                )}
              >
                {!isCompact && (
                  <div className="flex items-center justify-between w-full">
                    <span
                      className={cn("text-sm truncate max-w-[160px] relative", {
                        "text-primary-foreground": isActive,
                        "text-primary-foreground/80 group-hover:text-primary-foreground":
                          !isActive,
                      })}
                    >
                      {item.title}
                      {!isActive && (
                        <span
                          className="absolute right-0 top-0 h-full w-8 bg-gradient-to-l from-primary to-transparent"
                          aria-hidden="true"
                        />
                      )}
                    </span>
                    {isActive && (
                      <span className="flex h-2 w-2">
                        <span className="animate-ping absolute inline-flex h-2 w-2 rounded-full bg-blue-400 opacity-75"></span>
                        <span className="relative inline-flex rounded-full h-2 w-2 bg-blue-500"></span>
                      </span>
                    )}
                  </div>
                )}
              </Link>
            );

            return (
              <div key={item.id}>
                {isCompact ? (
                  <Tooltip content={item.title} placement="right">
                    {historyItemContent}
                  </Tooltip>
                ) : (
                  historyItemContent
                )}
              </div>
            );
          })}

          {!isCompact && hasMore && (
            <Button
              size="sm"
              //@ts-ignore
              variant=""
              className="w-full text-primary-foreground/60 hover:text-primary-foreground"
              isLoading={isLoadingMore}
              onPress={handleLoadMore}
              isDisabled={isLoadingMore}
              endContent={<ChevronDown className="mt-[3px]" size={15} />}
            >
              Show more
            </Button>
          )}
        </>
      );
    };

    return (
      <nav
        ref={ref}
        className={cn(
          "flex flex-col gap-2 flex-grow overflow-y-auto scrollbar-hide",
          className
        )}
      >
        {sidebarItems.map((item) => {
          const isActive = false;

          const itemContent = (
            <Link
              to={item.href || ""}
              className={cn(
                "flex items-center gap-2 px-3 min-h-11 rounded-large h-[44px] transition-colors",
                {
                  "w-11 h-10 justify-center p-0": isCompact,
                  "bg-[#333333] text-primary-foreground": isActive,
                  "hover:bg-primary-800/30 text-primary-foreground/80 hover:text-primary-foreground":
                    !isActive,
                }
              )}
            >
              <DynamicIcon
                size={22}
                name={item.iconName}
                className={cn({
                  "text-primary-foreground": isActive,
                  "text-primary-foreground/80 group-hover:text-primary-foreground":
                    !isActive,
                })}
              />
              {!isCompact && (
                <span
                  className={cn("text-sm", {
                    "text-primary-foreground": isActive,
                    "text-primary-foreground/80 group-hover:text-primary-foreground":
                      !isActive,
                  })}
                >
                  {item.title}
                </span>
              )}
            </Link>
          );

          return (
            <div key={item.key}>
              {isCompact ? (
                <Tooltip content={item.title} placement="right">
                  {itemContent}
                </Tooltip>
              ) : (
                itemContent
              )}
            </div>
          );
        })}

        {showRecentChats && historyItems.length > 0 && (
          <div className="h-px bg-primary-800/30" aria-hidden="true" />
        )}

        {renderHistoryItems()}
      </nav>
    );
  }
);

SidebarNav.displayName = "SidebarNav";

export default SidebarNav;
