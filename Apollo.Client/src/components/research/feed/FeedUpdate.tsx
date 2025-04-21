import { Search } from "lucide-react";
import type { FeedUpdate, SearchResultItem } from "../../../lib/types/research";

interface ResearchFeedUpdateProps {
  update: FeedUpdate;
}

export function MessageUpdate({ content }: { content: string }) {
  return (
    <div className="flex items-start">
      <span className="mr-2 mt-1 text-lg">•</span>
      <p className="text-gray-700">{content}</p>
    </div>
  );
}

export function SearchingUpdate({ query }: { query: string }) {
  return (
    <div className="flex items-center gap-2">
      <Search className="h-4 w-4 text-gray-500" />
      <p className="text-gray-700">
        Searching for <span className="font-medium">"{query}"</span>
      </p>
    </div>
  );
}

export function SearchResultsUpdate({
  results,
}: {
  results: SearchResultItem[];
}) {
  return (
    <div className="space-y-3">
      {results.map((result) => (
        <SearchResult key={result.id} {...result} />
      ))}
    </div>
  );
}

export function SnippetUpdate({
  content,
  highlights = [],
}: {
  content: string;
  highlights?: string[];
}) {
  // If we have highlights, wrap them in highlight spans
  if (highlights.length > 0) {
    let highlightedContent = content;
    highlights.forEach((highlight) => {
      highlightedContent = highlightedContent.replace(
        new RegExp(highlight, "gi"),
        `<span class="bg-yellow-100 px-1 rounded">${highlight}</span>`
      );
    });
    return (
      <div className="bg-gray-50 p-3 rounded-lg">
        <p
          className="text-gray-700"
          dangerouslySetInnerHTML={{ __html: highlightedContent }}
        />
      </div>
    );
  }

  return (
    <div className="bg-gray-50 p-3 rounded-lg">
      <p className="text-gray-700">{content}</p>
    </div>
  );
}

function SearchResult({
  icon,
  title,
  url,
  snippet,
  highlights = [],
}: SearchResultItem) {
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
        {snippet && (
          <p className="mt-2 text-sm text-gray-600">
            {highlights.length > 0 ? (
              <span
                dangerouslySetInnerHTML={{
                  __html: highlights.reduce(
                    (acc, highlight) =>
                      acc.replace(
                        new RegExp(highlight, "gi"),
                        `<span class="bg-yellow-100 px-1 rounded">${highlight}</span>`
                      ),
                    snippet
                  ),
                }}
              />
            ) : (
              snippet
            )}
          </p>
        )}
      </div>
    </div>
  );
}

export default function ResearchFeedUpdate({
  update,
}: ResearchFeedUpdateProps) {
  switch (update.type) {
    case "message":
      return <MessageUpdate content={update.content} />;
    case "searching":
      return <SearchingUpdate query={update.query || ""} />;
    case "search_results":
      return <SearchResultsUpdate results={update.searchResults || []} />;
    case "snippet":
      return (
        <SnippetUpdate
          content={update.content}
          highlights={update.highlights}
        />
      );
    default:
      return null;
  }
}
