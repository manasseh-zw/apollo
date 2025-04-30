import { useState, useEffect, useRef } from "react";
import { Avatar, AvatarGroup } from "@heroui/react";
import {
  type TimelineItem,
  type ResearchFeedUpdate,
  type TimelineUpdate,
  type ResearchResponse,
  TimelineItemStatus,
  TimelineItemType,
  ResearchFeedUpdateType,
  type SearchResultsUpdate,
} from "../../../lib/types/research";
import ResearchFeedUpdateComponent from "./FeedUpdate";
import TriLoader from "./TriLoader";
import { useResearchTimer } from "../../../lib/hooks/useResearchTimer";
import type { HubConnection } from "@microsoft/signalr";
import React from "react";
import VerticalTimeline from "./VerticalTimeline";

interface ResearchFeedProps {
  connection: HubConnection | null;
  researchId: string;
  research: ResearchResponse;
  initialFeedUpdates?: ResearchFeedUpdate[];
}

export default function ResearchFeed({
  connection,
  research,
  initialFeedUpdates,
}: ResearchFeedProps) {
  //   const [expanded, setExpanded] = useState(false);
  //   const [isLoading, setIsLoading] = useState(false);
  const [error] = useState<string | null>(null);
  const [feedUpdates, setFeedUpdates] = useState<ResearchFeedUpdate[]>(
    initialFeedUpdates ?? []
  );
  const [timelineItems, setTimelineItems] = useState<TimelineItem[]>(
    research.plan.questions.map((q, i) => ({
      id: i.toString(),
      text: q,
      type: TimelineItemType.Question,
      active: i === 0,
      status:
        i === 0 ? TimelineItemStatus.InProgress : TimelineItemStatus.Pending,
    }))
  );
  const feedEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    feedEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    if (!connection) return;

    connection.on("ReceiveResearchFeedUpdate", (update: ResearchFeedUpdate) => {
      setFeedUpdates((prev) => [...prev, update]);
    });

    connection.on("ReceiveTimelineUpdate", (update: TimelineUpdate) => {
      setTimelineItems(update.items);
    });

    return () => {
      connection.off("ReceiveResearchFeedUpdate");
      connection.off("ReceiveTimelineUpdate");
    };
  }, [connection]);

  useEffect(() => {
    scrollToBottom();
  }, [feedUpdates]);

  const elapsedTime = useResearchTimer(research.startedAt);

  const searchResultIcons = React.useMemo(() => {
    const icons = new Set<string>();
    feedUpdates.forEach((update) => {
      if (
        update.type === ResearchFeedUpdateType.SearchResults &&
        "results" in update // Type guard to ensure update is SearchResultsUpdate
      ) {
        const searchUpdate = update as SearchResultsUpdate;
        searchUpdate.results?.forEach((result) => {
          if (result?.icon) {
            icons.add(result.icon);
          }
        });
      }
    });
    return Array.from(icons);
  }, [feedUpdates]);

  if (error) {
    return (
      <div className="flex h-full w-full items-center justify-center">
        <p className="text-danger-500">Error: {error}</p>
      </div>
    );
  }

  return (
    <div className="flex h-full w-full overflow-hidden bg-white font-geist">
      <div className="w-[317px] border-r border-gray-200 bg-content p-5 flex flex-col h-full">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <TriLoader />
            <h2 className="text-lg text-gray-800">Deep Research</h2>
          </div>
          <p className="text-sm">{elapsedTime}</p>
        </div>
        <small className="mt-3">{research.title}</small>

        {/* Timeline section with scrollable content */}
        <div className="mt-6 flex-1 flex flex-col overflow-hidden ">
          <div className="flex-1 overflow-y-auto">
            <div className="relative">
              <div className="mb-8 flex flex-col gap-6">
                <VerticalTimeline
                  items={timelineItems}
                  color="primary" // Or "secondary", "success", etc.
                  // Optional: uncomment to hide lines
                  // className="my-custom-timeline-styles" // Optional: for wrapper styles
                  // itemClassName="my-custom-item-styles" // Optional: for individual item styles
                />
              </div>
            </div>
          </div>

          {/* Search Sources Avatar Group */}
          <div className="mt-4 pt-4 border-t border-gray-200">
            <div className="flex items-center justify-between">
              <span className="text-sm text-gray-600">Sources</span>
              <AvatarGroup
                size="sm"
                max={4}
                total={searchResultIcons.length}
                renderCount={(count) => (
                  <p className="text-small text-foreground font-medium ms-2 ">
                    +{count} others
                  </p>
                )}
              >
                {searchResultIcons
                  .slice(0, 5)
                  .map((icon: string, index: number) => (
                    <Avatar
                      classNames={{ base: "bg-white " }}
                      key={index}
                      src={icon}
                      fallback={
                        <img
                          src="https://www.google.com/favicon.ico"
                          alt="fallback"
                        />
                      }
                    />
                  ))}
              </AvatarGroup>
            </div>
          </div>
        </div>
      </div>

      <div className="flex-1 relative">
        <div className="absolute top-4 right-4 flex items-center gap-2 bg-content2 bg-opacity-80 backdrop-blur-sm px-3 py-1.5 rounded-full z-10">
          <span className="text-sm text-primary">Live Feed</span>
          <div className="relative flex">
            <span className="w-2 h-2 rounded-full bg-green-500" />
            <span className="absolute w-2 h-2 rounded-full bg-green-500 animate-ping" />
          </div>
        </div>

        <div className="absolute inset-0 overflow-y-auto overflow-x-hidden p-5">
          <div className="space-y-6">
            {feedUpdates.map((update, index) => (
              <ResearchFeedUpdateComponent
                key={`${update.timestamp}-${index}`}
                update={update}
              />
            ))}
            <div ref={feedEndRef} />
          </div>
        </div>
      </div>
    </div>
  );
}
