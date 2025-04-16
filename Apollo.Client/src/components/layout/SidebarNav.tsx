import React from "react";
import { cn } from "@heroui/react";
import { Link, useRouterState } from "@tanstack/react-router";
import { Tooltip } from "@heroui/react";
import { DynamicIcon, type IconName } from "lucide-react/dynamic";

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
    href: "/library",
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
    const activeRoute = useRouterState({
      select: (s) => s.location.pathname.split("/")[1],
    });

    return (
      <nav ref={ref} className={cn("flex flex-col gap-2", className)}>
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
      </nav>
    );
  }
);

export default SidebarNav;
