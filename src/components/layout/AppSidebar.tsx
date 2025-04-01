import {
  Avatar as UserAvatar,
  Button,
  cn,
  Spacer,
  Tooltip,
  Chip,
} from "@heroui/react";
import React from "react";
import { LogoLight } from "../../components/Icons";
import { useMediaQuery } from "usehooks-ts";
import { ChevronLeft, ChevronRight, Menu, MinusCircle } from "lucide-react";
import Avatar from "boring-avatars";
import { store } from "../../lib/state/store";
import SidebarDrawer from "./SidebarDrawer";
import SidebarNav from "./SidebarNav";

const SIDEBAR_COLLAPSED_KEY = "apolllo-sidebar-collapsed";

export default function AppSidebar() {
  const user = store.state.authState.user;
  const [isOpen, setIsOpen] = React.useState(false);

  const [isCollapsed, setIsCollapsed] = React.useState<boolean>(() => {
    if (typeof window !== "undefined") {
      const savedState = localStorage.getItem(SIDEBAR_COLLAPSED_KEY);
      return savedState ? JSON.parse(savedState) : false;
    }
    return false;
  });

  const isMobile = useMediaQuery("(max-width: 640px)");

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
        "will-change relative flex h-full w-[17rem] flex-col bg-content2 py-6 px-4 transition-width",
        {
          "w-[83px] items-center px-[6px] py-6": isCollapsed,
        }
      )}
    >
      <div
        className={cn("flex items-center justify-between gap-2 pl-2", {
          "justify-center gap-0 pl-0": isCollapsed,
        })}
      >
        <div className="flex items-center justify-center rounded-full">
          <LogoLight width={40} height={40} />
        </div>
        <span
          className={cn("w-full ", {
            "w-0 opacity-0": isCollapsed,
          })}
        >
          <Chip color="primary" className="border-1 border-transparent shadow  " variant="flat" size="sm">
            Deep Research
          </Chip>
        </span>
        <div className={cn("flex-end flex", { hidden: isCollapsed })}>
          <ChevronLeft
            className="cursor-pointer text-primary/80"
            size={24}
            onClick={isMobile ? () => setIsOpen(false) : onToggle}
          />
        </div>
      </div>

      <Spacer y={6} />
      <div className="flex items-center gap-3 px-3">
        {user.avatarUrl.length > 0 ? (
          <UserAvatar
            name={user.username.substring(0, 4)}
            src={user.avatarUrl}
            size="sm"
            isBordered
            color="success"
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
            className="text-small  text-foreground truncate"
            title={user.username}
          >
            {user.username.substring(0, 25)}
            {user.username.length > 25 ? "..." : ""}
          </p>
          <p className="text-tiny  text-foreground-300">Researcher</p>
        </div>
      </div>

      <Spacer y={6} />

      <SidebarNav isCompact={isCollapsed} />

      <Spacer y={8} />

      <div
        className={cn("mt-auto flex flex-col", {
          "items-center": isCollapsed,
        })}
      >
        {isCollapsed && (
          <Button
            isIconOnly
            className="flex h-10 w-10 text-default-600"
            size="sm"
            variant="light"
            onClick={onToggle}
          >
            <ChevronRight
              className="cursor-pointer text-primary/80"
              size={24}
            />
          </Button>
        )}

        <Tooltip content="Log Out" isDisabled={!isCollapsed} placement="right">
          <button
            className={cn(
              "flex items-center gap-2 px-3 min-h-11 rounded-large h-[44px] transition-colors hover:bg-default-100/50",
              {
                "justify-center w-11 h-11 p-0": isCollapsed,
              }
            )}
          >
            <MinusCircle className="rotate-180 text-primary" size={24} />
            {!isCollapsed && (
              <span className="text-small  text-primary">Log Out</span>
            )}
          </button>
        </Tooltip>
      </div>
    </div>
  );

  return (
    <>
      {isMobile && !isOpen && (
        <Button
          isIconOnly
          className="fixed top-4 left-4 z-50 bg-content2 text-gray-700 md:hidden"
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
            "min-w-[76px]": isCollapsed,
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
            "min-w-[76px]": isCollapsed,
          })}
        >
          <SidebarContent />
        </div>
      )}
    </>
  );
}
