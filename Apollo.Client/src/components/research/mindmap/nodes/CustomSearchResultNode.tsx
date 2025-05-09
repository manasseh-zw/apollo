import { Handle, Position } from "@xyflow/react";
import { Popover, PopoverTrigger, PopoverContent, Image } from "@heroui/react";
import type { SearchResultMindMapNode } from "../../../../lib/types/research";

interface CustomSearchResultNodeProps {
  data: SearchResultMindMapNode;
}

export default function CustomSearchResultNode({
  data,
}: CustomSearchResultNodeProps) {
  // Extract domain from URL
  const domain = data.url.replace(/^https?:\/\//, "").split("/")[0];

  return (
    <div className="relative">
      <Handle type="target" position={Position.Right} />
      <Popover placement="right">
        <PopoverTrigger>
          <div className="px-4 py-2 shadow-lg rounded-lg bg-white hover:bg-content1 transition-colors cursor-pointer min-w-[140px]">
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 flex-shrink-0">
                <img
                  src={data.favicon}
                  alt=""
                  className="w-full h-full object-contain"
                  onError={(e) => {
                    e.currentTarget.src = "https://www.google.com/favicon.ico";
                  }}
                />
              </div>
              <div>
                <div className="font-medium text-sm line-clamp-1">
                  {data.title}
                </div>
                <div className="text-xs text-default-500">{domain}</div>
              </div>
            </div>
          </div>
        </PopoverTrigger>
        <PopoverContent className="w-[300px]">
          {(titleProps) => (
            <div className="p-2">
              <div className="font-medium mb-2" {...titleProps}>
                {data.title}
              </div>
              {data.imageUrl && (
                <div className="mb-2">
                  <Image
                    src={data.imageUrl}
                    alt={data.title}
                    className="w-full h-32 object-cover rounded"
                  />
                </div>
              )}
              <p className="text-sm text-default-700">{data.summary}</p>
              <a
                href={data.url}
                target="_blank"
                rel="noopener noreferrer"
                className="text-xs text-primary hover:underline mt-2 block"
              >
                {domain}
              </a>
            </div>
          )}
        </PopoverContent>
      </Popover>
    </div>
  );
}
