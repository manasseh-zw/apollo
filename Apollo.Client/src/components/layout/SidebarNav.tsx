import React from "react";
import { cn } from "@heroui/react";
import { Link, useRouterState } from "@tanstack/react-router";
import { Tooltip, Skeleton, Button } from "@heroui/react";
import { DynamicIcon, type IconName } from "lucide-react/dynamic";
import { apiRequest } from "../../lib/utils/api";
import type {
  ResearchHistoryItem,
  PaginatedResponse,
} from "../../lib/types/research";

export type SidebarItem = {
  key: string;
  title: string;
  iconName: IconName;
  href?: string;
};

export const sidebarItems: SidebarItem[] = [
  {
    key: "research",
    href: "/research",
    iconName: "telescope",
    title: "Research",
  },
  {
    key: "history",
    href: "/history",
    iconName: "history",
    title: "History",
  },
];

export type SidebarProps = {
  isCompact?: boolean;
  className?: string;
};

const SidebarNav = React.forwardRef<HTMLElement, SidebarProps>(
  ({ isCompact, className }, ref) => {
    const [historyItems, setHistoryItems] = React.useState<
      ResearchHistoryItem[]
    >([]);
    const [isLoadingInitial, setIsLoadingInitial] = React.useState(true);
    const [isLoadingMore, setIsLoadingMore] = React.useState(false);
    const [error, setError] = React.useState<string | null>(null);
    const [currentPage, setCurrentPage] = React.useState(1);
    const [hasMore, setHasMore] = React.useState(false);

    const pathname = useRouterState({
      select: (s) => s.location.pathname,
    });

    const activeRoute = pathname.split("/")[1];
    const activeResearchId = pathname.startsWith("/research/")
      ? pathname.split("/")[2]
      : null;

    const fetchHistory = React.useCallback(async (page: number) => {
      const isInitialFetch = page === 1;
      try {
        if (isInitialFetch) {
          setIsLoadingInitial(true);
        } else {
          setIsLoadingMore(true);
        }

        const response = await apiRequest<
          PaginatedResponse<ResearchHistoryItem>
        >(`/api/research/history?page=${page}&pageSize=5`);

        if (response.success && response.data) {
          if (isInitialFetch) {
            setHistoryItems(response.data.items);
          } else {
            //@ts-ignore
            setHistoryItems((prev) => [...prev, ...response.data.items]);
          }
          setHasMore(response.data.hasMore);
          setCurrentPage(page);
        } else {
          setError("Failed to load research history");
        }
      } catch (err) {
        setError("Failed to load research history");
      } finally {
        if (isInitialFetch) {
          setIsLoadingInitial(false);
        } else {
          setIsLoadingMore(false);
        }
      }
    }, []);

    React.useEffect(() => {
      fetchHistory(1);
    }, [fetchHistory]);

    const handleLoadMore = () => {
      if (!isLoadingMore && hasMore) {
        fetchHistory(currentPage + 1);
      }
    };

    const renderHistoryItems = () => {
      if (isLoadingInitial) {
        return Array(3)
          .fill(0)
          .map((_, i) => (
            <div key={i} className="px-2 min-h-5  flex items-center">
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
                        className="absolute right-0 top-0 h-full w-8 bg-gradient-to-l from-primary  to-transparent"
                        aria-hidden="true"
                      />
                    )}
                  </span>
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
                  <Tooltip content={item.title} placement="right">
                    {historyItemContent}
                  </Tooltip>
                )}
              </div>
            );
          })}

          {!isCompact && hasMore && (
            <Button
              size="sm"
              variant="light"
              className="w-full text-primary-foreground/60 hover:text-primary-foreground"
              isLoading={isLoadingMore}
              onPress={handleLoadMore}
              isDisabled={isLoadingMore}
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
          const isActive = activeRoute === item.key;

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

        {!isCompact && historyItems.length > 0 && (
          <div className="h-px bg-primary-800/30 " aria-hidden="true" />
        )}

        {renderHistoryItems()}
      </nav>
    );
  }
);

SidebarNav.displayName = "SidebarNav";

export default SidebarNav;
