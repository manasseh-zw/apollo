import { Search } from "lucide-react";
import {
  type ResearchFeedUpdate,
  type SearchResultItem,
  ResearchFeedUpdateType,
} from "../../../lib/types/research";

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
  highlights?: string[] | undefined;
}) {
  if (highlights.length && highlights.length > 0) {
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
  // Extract domain from URL
  const domain = url.replace(/^https?:\/\//, "").split("/")[0];

  return (
    <div className="flex items-start gap-3 group cursor-pointer hover:bg-gray-50 p-2 rounded-lg transition-colors">
      <div className="flex h-6 w-6 pt-1 items-center justify-center overflow-hidden rounded">
        <img
          src={icon}
          alt={`${title} favicon`}
          className="h-full w-full object-contain"
          onError={(e) => {
            e.currentTarget.src = "https://www.google.com/favicon.ico";
          }}
        />
      </div>
      <div className="flex-1">
        <div className="flex items-center gap-1">
          <p className="font-medium text-primary group-hover:text-primary transition-colors">
            {title}
          </p>
        </div>
        <a className="text-sm text-blue-500" href={url}>
          {domain}
        </a>
        {snippet && (
          <div className="relative mt-3 pl-4 prose">
            <div className="absolute left-0 top-0 w-[1px] h-full bg-content2 rounded-full"></div>
            <p className="text-sm text-gray-600 tracking-wide leading-relaxed">
              {highlights.length && highlights.length > 0 ? (
                <span
                  className="[&>*]:tracking-normal"
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
          </div>
        )}
      </div>
    </div>
  );
}

export default function ResearchFeedUpdateComponent({
  update,
}: {
  update: ResearchFeedUpdate;
}) {
  switch (update.type) {
    case ResearchFeedUpdateType.Message:
      return <MessageUpdate content={update.message} />;
    case ResearchFeedUpdateType.Searching:
      return <SearchingUpdate query={update.query || ""} />;
    case ResearchFeedUpdateType.SearchResults:
      return <SearchResultsUpdate results={update.results || []} />;
    case ResearchFeedUpdateType.Snippet:
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
