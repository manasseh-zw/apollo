"use client";

import { useState, useEffect } from "react";
import { Check, ChevronDown, Globe } from "lucide-react";
import { Checkbox, Spinner, Avatar, AvatarGroup } from "@heroui/react";
import type {
  TimelineItem,
  SearchResultItem,
  ResearchProgress,
  FeedUpdate,
  ResearchSession,
} from "../../../lib/types/research";
import ResearchFeedUpdate from "./FeedUpdate";
import TriLoader from "./TriLoader";
import { useResearchTimer } from "../../../lib/hooks/useResearchTimer";

export default function ResearchFeed() {
  const [expanded, setExpanded] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const mockSession: ResearchSession = {
    id: "1",
    startedAt: new Date(Date.now() - 1000 * 60 * 3).toISOString(), // 3 minutes ago
    status: "active",
    sourcesCount: 13,
  };

  const elapsedTime = useResearchTimer(mockSession.startedAt);

  const progress: ResearchProgress = {
    duration: elapsedTime,
    sourcesCount: mockSession.sourcesCount,
    webPagesCount: 13,
    status: "completed",
  };

  const timelineItems: TimelineItem[] = [
    { id: "1", text: "Thinking", status: "completed", active: true },
    { id: "2", text: "Exploring Atlantis inquiry", status: "completed" },
    { id: "3", text: "Evaluating Atlantis connections", status: "in_progress" },
    { id: "4", text: "Assessing cultural relevance", status: "pending" },
    { id: "5", text: "Refining Atlantis answer", status: "pending" },
    { id: "6", text: "Researching Atlantis", status: "pending" },
  ];

  const searchResults: SearchResultItem[] = [
    {
      id: "1",
      icon: "W",
      title: "Atlantis - Wikipedia",
      url: "en.wikipedia.org",
    },
    {
      id: "2",
      icon: "NG",
      title: "Atlantis Legend | National Geographic",
      url: "nationalgeographic.com",
      isNatGeo: true,
    },
    {
      id: "3",
      icon: "W",
      title: "Location hypotheses of Atlantis - Wikipedia",
      url: "en.wikipedia.org",
    },
    {
      id: "4",
      icon: "LS",
      title: "'Lost' City of Atlantis: Fact & Fable | Live Science",
      url: "livescience.com",
      isLiveScience: true,
    },
    {
      id: "5",
      icon: "H",
      title: "Atlantis",
      url: "history.com",
      isHistory: true,
    },
  ];

  const feedUpdates: FeedUpdate[] = [
    {
      id: "1",
      type: "message",
      content:
        "The request is about Atlantis, focusing on its origins, evidence of existence, and historical/cultural significance.",
      timestamp: new Date().toISOString(),
    },
    {
      id: "2",
      type: "searching",
      content: "",
      query: "Atlantis origin and evidence",
      timestamp: new Date().toISOString(),
    },
    {
      id: "3",
      type: "search_results",
      content: "",
      searchResults: searchResults,
      timestamp: new Date().toISOString(),
    },
    {
      id: "4",
      type: "snippet",
      content:
        'Atlantis is a legendary island from Plato\'s dialogues "Timaeus" and "Critias," described as a powerful civilization 9,000 years before his time, located beyond the Strait of Gibraltar.',
      highlights: ["Plato", "Timaeus", "Critias", "civilization"],
      timestamp: new Date().toISOString(),
    },
  ];

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
          <h2 className="text-xl text-gray-800">
            {progress.status === "completed" ? "Deep Research" : "In Progress"}
          </h2>
        </div>
        <p className="mt-2 text-sm text-gray-500">
          {elapsedTime} · {progress.sourcesCount} sources
        </p>

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

        <WebSourcesGroup sources={searchResults} />
      </div>

      <div className="flex-1 relative">
        <div className="absolute inset-0 overflow-y-auto p-5">
          <h3 className="mb-4 text-lg font-medium text-gray-800">
            Exploring Atlantis inquiry
          </h3>

          <div className="space-y-6">
            {feedUpdates.map((update) => (
              <ResearchFeedUpdate key={update.id} update={update} />
            ))}
          </div>

          {expanded ? (
            <button
              onClick={() => setExpanded(false)}
              className="mt-4 text-gray-600 hover:text-gray-800 flex items-center gap-1"
            >
              Show less <ChevronDown className="h-4 w-4 transform rotate-180" />
            </button>
          ) : (
            <button
              onClick={() => setExpanded(true)}
              className="mt-4 text-gray-600 hover:text-gray-800 flex items-center gap-1"
            >
              See more <ChevronDown className="h-4 w-4" />
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

function WebSourcesGroup({ sources }: { sources: SearchResultItem[] }) {
  return (
    <div className="mt-4 flex items-center gap-2 text-sm text-gray-700 border-t border-gray-100 pt-4">
      <AvatarGroup
        size="sm"
        max={3}
        className="justify-start"
        renderCount={(count) => (
          <p className="text-small text-gray-600 font-medium ms-2">
            +{count} more sources
          </p>
        )}
        total={sources.length}
      >
        {sources.map((source) => (
          <Avatar
            size="sm"
            key={source.id}
            name={source.icon}
            className="bg-content2 text-tiny font-medium"
            classNames={{
              base: source.icon === "W" ? "bg-white" : "",
              name: source.icon === "W" ? "font-serif italic" : "",
            }}
          />
        ))}
      </AvatarGroup>
    </div>
  );
}

function TimelineItem({ text, active = false, status }: TimelineItem) {
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

function SearchResult({ icon, title, url }: SearchResultItem) {
  return (
    <div className="flex items-start gap-3 group cursor-pointer hover:bg-gray-50 p-2 rounded-lg transition-colors">
      <div className="flex h-6 w-6 items-center justify-center">
        {icon === "W" ? (
          <span className="font-serif italic text-gray-700">W</span>
        ) : icon === "NG" ? (
          <div className="flex h-5 w-5 items-center justify-center bg-yellow-500 text-[10px] font-bold text-black">
            {icon}
          </div>
        ) : icon === "LS" ? (
          <div className="flex h-5 w-5 items-center justify-center rounded-full bg-orange-400 text-[10px] font-bold text-white">
            {icon}
          </div>
        ) : icon === "H" ? (
          <div className="flex h-5 w-5 items-center justify-center bg-yellow-500 text-[10px] font-bold text-black">
            {icon}
          </div>
        ) : (
          icon
        )}
      </div>
      <div>
        <div className="flex items-center gap-1">
          <p className="font-medium text-gray-800 group-hover:text-primary transition-colors">
            {title}
          </p>
        </div>
        <p className="text-sm text-gray-500">{url}</p>
      </div>
    </div>
  );
}
