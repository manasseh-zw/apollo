import { Handle } from "@xyflow/react";
import type { SearchQueryMindMapNode } from "../../../../lib/types/research";
import { Search } from "lucide-react";

interface CustomSearchQueryNodeProps {
  data: SearchQueryMindMapNode;
}

export default function CustomSearchQueryNode({ data }: CustomSearchQueryNodeProps) {
  return (
    <div className="px-4 py-2 shadow-lg rounded-lg bg-content3 min-w-[160px]">
      <Handle type="target" />
      <Handle type="source" />
      <div className="flex items-center gap-2">
        <Search className="w-4 h-4 text-default-500" />
        <div className="font-medium text-sm">{data.queryText}</div>
      </div>
      <div className="text-xs mt-1 text-default-500">
        {new Date(data.executedAt).toLocaleTimeString()}
      </div>
    </div>
  );
}