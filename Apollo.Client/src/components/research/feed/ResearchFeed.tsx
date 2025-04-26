"use client";

import { useState, useEffect } from "react";
import { Check } from "lucide-react";
import { Spinner } from "@heroui/react";
import type {
  QuestionTimelineItem,
  ResearchFeedUpdate,
  QuestionTimelineUpdate,
  ResearchResponse,
} from "../../../lib/types/research";
import ResearchFeedUpdateComponent from "./FeedUpdate";
import TriLoader from "./TriLoader";
import { useResearchTimer } from "../../../lib/hooks/useResearchTimer";
import { HubConnection } from "@microsoft/signalr";

interface ResearchFeedProps {
  connection: HubConnection | null;
  researchId: string;
  research: ResearchResponse;
}

export default function ResearchFeed({
  connection,
  researchId,
  research,
}: ResearchFeedProps) {
  const [expanded, setExpanded] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [feedUpdates, setFeedUpdates] = useState<ResearchFeedUpdate[]>([]);
  const [timelineItems, setTimelineItems] = useState<QuestionTimelineItem[]>(
    research.plan.questions.map((q, i) => ({
      id: i.toString(),
      text: q,
      active: i === 0,
      status: i === 0 ? "in_progress" : "pending",
    }))
  );

  const elapsedTime = useResearchTimer(research.startedAt);

  useEffect(() => {
    if (!connection) return;

    connection.on("ReceiveResearchFeedUpdate", (update: ResearchFeedUpdate) => {
      setFeedUpdates((prev) => [...prev, update]);
    });

    connection.on(
      "ReceiveQuestionTimelineUpdate",
      (update: QuestionTimelineUpdate) => {
        setTimelineItems(update.questions);
      }
    );

    return () => {
      connection.off("ReceiveResearchFeedUpdate");
      connection.off("ReceiveQuestionTimelineUpdate");
    };
  }, [connection]);

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
        <div className="flex items-center gap-2">
          <TriLoader />
          <h2 className="text-xl text-gray-800">{research.title}</h2>
        </div>
        <p className="mt-2 text-sm text-gray-500">{elapsedTime}</p>

        <div className="mt-6 flex-1 overflow-y-auto">
          <div className="relative">
            <div className="absolute mt-1 left-[10px] top-0 h-full w-[2px] bg-gray-300"></div>

            <div className="mb-8 flex flex-col gap-6">
              {timelineItems.map((item) => (
                <TimelineItem key={item.id} {...item} />
              ))}
            </div>
          </div>
        </div>
      </div>

      <div className="flex-1 relative">
        <div className="absolute inset-0 overflow-y-auto p-5">
          <div className="space-y-6">
            {feedUpdates.map((update, index) => (
              <ResearchFeedUpdateComponent
                key={`${update.timestamp}-${index}`}
                update={update}
              />
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}

function TimelineItem({ text, active = false, status }: QuestionTimelineItem) {
  return (
    <div className="relative flex items-start gap-4">
      {status === "completed" ? (
        <div className="rounded-full bg-primary p-1">
          <Check className="h-4 w-4 text-white" />
        </div>
      ) : status === "in_progress" ? (
        <Spinner size="sm" className="bg-white" />
      ) : (
        <div className="h-6 w-6 rounded-full border-2 border-gray-300 bg-white" />
      )}
      <span
        className={`text-sm ${active ? "text-primary font-medium" : "text-gray-600"}`}
      >
        {text}
      </span>
    </div>
  );
}
