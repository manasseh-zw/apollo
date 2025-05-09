import { Handle, Position } from "@xyflow/react";
import type { QuestionMindMapNode } from "../../../../lib/types/research";

interface CustomQuestionNodeProps {
  data: QuestionMindMapNode;
}

export default function CustomQuestionNode({ data }: CustomQuestionNodeProps) {
  return (
    <div className={`px-4 py-2 shadow-lg rounded-lg min-w-[180px] ${
      data.isGapQuestion ? 'bg-secondary text-secondary-foreground' : 'bg-content2'
    }`}>
      <Handle type="target" position={Position.Left} />
      <Handle type="source" position={Position.Right} />
      <div className="font-medium">{data.questionText}</div>
      {data.isGapQuestion && (
        <div className="text-xs mt-1 text-secondary-foreground/80">Gap Question</div>
      )}
    </div>
  );
}