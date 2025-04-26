import {
  Avatar as UserAvatar,
  Button,
  cn,
  Spacer,
  Tooltip,
  useDisclosure,
} from "@heroui/react";
import React from "react";
import { LogoLight } from "../../components/Icons";
import { useMediaQuery } from "usehooks-ts";
import {
  ChevronLeft,
  ChevronRight,
  History,
  LogOut,
  Menu,
  Telescope,
} from "lucide-react";
import Avatar from "boring-avatars";
import { store } from "../../lib/state/store";
import SidebarDrawer from "./SidebarDrawer";
import SidebarNav from "./SidebarNav";
import { Link, useRouterState } from "@tanstack/react-router";
import { apiRequest } from "../../lib/utils/api";
import type {
  ResearchHistoryItem,
  PaginatedResponse,
} from "../../lib/types/research";
import HistoryModalTrigger from "./HistoryModal";

const SIDEBAR_COLLAPSED_KEY = "apolllo-sidebar-collapsed";

export default function AppSidebar() {
  const user = store.state.authState.user;
  const [isOpen, setIsOpen] = React.useState(false);
  const {
    isOpen: isHistoryOpen,
    onOpen: onHistoryOpen,
    onOpenChange: onHistoryOpenChange,
  } = useDisclosure();

  const [historyItems, setHistoryItems] = React.useState<ResearchHistoryItem[]>(
    []
  );
  const [isLoadingInitial, setIsLoadingInitial] = React.useState(true);
  const [isLoadingMore, setIsLoadingMore] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);
  const [currentPage, setCurrentPage] = React.useState(1);
  const [hasMore, setHasMore] = React.useState(false);

  const [isCollapsed, setIsCollapsed] = React.useState<boolean>(() => {
    if (typeof window !== "undefined") {
      const savedState = localStorage.getItem(SIDEBAR_COLLAPSED_KEY);
      return savedState ? JSON.parse(savedState) : false;
    }
    return false;
  });

  const isMobile = useMediaQuery("(max-width: 640px)");
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

      const response = await apiRequest<PaginatedResponse<ResearchHistoryItem>>(
        `/api/research/history?page=${page}&pageSize=5`
      );

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

  const onToggle = React.useCallback(() => {
    const newState = !isCollapsed;
    setIsCollapsed(newState);
    if (typeof window !== "undefined") {
      localStorage.setItem(SIDEBAR_COLLAPSED_KEY, JSON.stringify(newState));
    }
  }, [isCollapsed]);

  React.useEffect(() => {
    if (isMobile) {
      setIsOpen(false);
      setIsCollapsed(false);
    }
  }, [isMobile]);

  const SidebarContent = () => (
    <div
      className={cn(
        "will-change relative flex h-full w-[17rem] flex-col bg-primary py-6 px-4 transition-width",
        {
          "w-[64px] items-center px-[4px] py-6": isCollapsed,
        }
      )}
    >
      <div
        className={cn("flex items-center justify-between gap-2 pl-2", {
          "justify-center gap-0 pl-0": isCollapsed,
        })}
      >
        <div className="flex items-center justify-center rounded-full gap-2 cursor-pointer">
          <LogoLight
            width={28}
            height={28}
            className="text-primary-foreground"
          />
          <span
            className={cn("w-full uppercase font-geist text-white", {
              hidden: isCollapsed,
            })}
          >
            Apollo
          </span>
        </div>

        <div className={cn("flex-end flex", { hidden: isCollapsed })}>
          <ChevronLeft
            className="cursor-pointer text-primary-foreground/80 hover:text-primary-foreground"
            size={24}
            onClick={isMobile ? () => setIsOpen(false) : onToggle}
          />
        </div>
      </div>

      <Spacer y={6} />
      <div className="flex items-center gap-3 px-3 cursor-pointer">
        {user.avatarUrl ? (
          <UserAvatar
            name={user.username.substring(0, 4)}
            src={user.avatarUrl}
            size="sm"
          />
        ) : (
          <Avatar size={38} name={user.username} />
        )}

        <div
          className={cn("flex max-w-full flex-col overflow-hidden", {
            hidden: isCollapsed,
          })}
        >
          <p
            className="text-small text-primary-foreground truncate"
            title={user.username}
          >
            {user.username.substring(0, 25)}
            {user.username.length > 25 ? "..." : ""}
          </p>
          <p className="text-tiny text-primary-foreground/80">Researcher</p>
        </div>
      </div>

      <Spacer y={6} />

      <div className="flex flex-col gap-2 mb-4">
        <Tooltip content="Research" isDisabled={!isCollapsed} placement="right">
          <Link
            to="/research"
            className={cn(
              "flex items-center gap-2 px-3 min-h-11 rounded-2xl h-[44px] transition-colors bg-white",
              {
                "w-11 h-10 justify-center p-0": isCollapsed,
                "bg-white text-primary": activeRoute === "research",
              }
            )}
          >
            <Telescope size={22} className={cn("text-primary")} />
            {!isCollapsed && (
              <span className="text-sm text-primary">Research</span>
            )}
          </Link>
        </Tooltip>

        {isCollapsed && (
          <Tooltip content="History" placement="right">
            <button
              onClick={onHistoryOpen}
              className="flex items-center justify-center w-11 h-10 p-0 rounded-large transition-colors hover:bg-primary-800/30"
            >
              <History
                size={22}
                className="text-primary-foreground/80 group-hover:text-primary-foreground"
              />
            </button>
          </Tooltip>
        )}
      </div>

      <SidebarNav
        isCompact={isCollapsed}
        historyItems={historyItems}
        isLoadingInitial={isLoadingInitial}
        error={error}
        hasMore={hasMore}
        handleLoadMore={handleLoadMore}
        activeResearchId={activeResearchId}
        showRecentChats={!isCollapsed}
      />

      <Spacer y={8} />

      <div
        className={cn("mt-auto flex flex-col", {
          "items-center": isCollapsed,
        })}
      >
        {isCollapsed && (
          <Button
            isIconOnly
            className="flex h-9 w-9 text-primary-foreground/80 hover:text-primary-foreground"
            size="sm"
            variant="light"
            onClick={onToggle}
          >
            <ChevronRight size={24} />
          </Button>
        )}

        <Tooltip content="Log Out" isDisabled={!isCollapsed} placement="right">
          <button
            className={cn(
              "flex items-center gap-2 px-3 min-h-11 rounded-large h-[44px] transition-colors hover:bg-default-100/50",
              {
                "justify-center w-10 h-10 p-0": isCollapsed,
              }
            )}
          >
            <LogOut
              className="rotate-180 text-primary-foreground/80 hover:text-primary-foreground"
              size={24}
            />
            {!isCollapsed && (
              <span className="text-small text-primary-foreground/80 hover:text-primary-foreground">
                Log Out
              </span>
            )}
          </button>
        </Tooltip>
      </div>

      <HistoryModalTrigger
        items={historyItems}
        isOpen={isHistoryOpen}
        onOpenChange={onHistoryOpenChange}
      />
    </div>
  );

  return (
    <>
      {isMobile && !isOpen && (
        <Button
          isIconOnly
          className="fixed top-4 left-4 z-50 text-primary bg-transparent md:hidden"
          size="sm"
          variant="flat"
          onPress={() => setIsOpen(true)}
        >
          <Menu size={24} />
        </Button>
      )}

      {isMobile ? (
        <SidebarDrawer
          className={cn("min-w-[17rem] rounded-lg overflow-hidden w-min", {
            "min-w-[64px]": isCollapsed,
          })}
          hideCloseButton={true}
          isOpen={isOpen}
          onOpenChange={setIsOpen}
        >
          <SidebarContent />
        </SidebarDrawer>
      ) : (
        <div
          className={cn("min-w-[17rem] h-full overflow-hidden md:block", {
            "min-w-[64px]": isCollapsed,
          })}
        >
          <SidebarContent />
        </div>
      )}
    </>
  );
}
