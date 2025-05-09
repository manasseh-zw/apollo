import { Handle, Position } from "@xyflow/react";
import type { RootMindMapNode } from "../../../../lib/types/research";

interface CustomRootNodeProps {
  data: RootMindMapNode;
}

export default function CustomRootNode({ data }: CustomRootNodeProps) {
  return (
    <div className="px-4 py-2 shadow-lg rounded-lg bg-primary text-primary-foreground min-w-[200px]">
      <Handle type="source" position={Position.Right} className="!bg-primary" />
      <div className="font-medium">{data.researchTitle}</div>
      <div className="text-sm text-primary-foreground/80 mt-1 line-clamp-2">
        {data.researchDescription}
      </div>
    </div>
  );
}
