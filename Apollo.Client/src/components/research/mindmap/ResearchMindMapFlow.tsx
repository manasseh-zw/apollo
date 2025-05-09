import {
  ReactFlowProvider,
  Controls,
  Background,
  type Node,
  type Edge,
} from "@xyflow/react";
import "@xyflow/react/dist/style.css";
import { useMemo } from "react";
import {
  type RootMindMapNode,
  type MindMapNode,
  MindMapNodeType,
} from "../../../lib/types/research";
import { ReactFlow } from "@xyflow/react";

// Import custom node components
import CustomRootNode from "./nodes/CustomRootNode";
import CustomQuestionNode from "./nodes/CustomQuestionNode";
import CustomSearchQueryNode from "./nodes/CustomSearchQueryNode";
import CustomSearchResultNode from "./nodes/CustomSearchResultNode";

interface ResearchMindMapFlowProps {
  mindMapData: RootMindMapNode;
}

// Node types mapping for React Flow
const nodeTypes = {
  rootNode: CustomRootNode,
  questionNode: CustomQuestionNode,
  searchQueryNode: CustomSearchQueryNode,
  searchResultNode: CustomSearchResultNode,
};

// Constants for radial layout
const R_QUESTIONS = 350; // Radius for questions around root
const R_QUERIES = 250; // Radius for queries around questions
const R_RESULTS = 180; // Radius for results around queries

// Helper function to create a unique edge ID
const createEdgeId = (source: string, target: string) => `${source}->${target}`;

// Helper function to get node type string based on MindMapNodeType
const getNodeType = (type: MindMapNodeType): string => {
  switch (type) {
    case MindMapNodeType.Root:
      return "rootNode";
    case MindMapNodeType.Question:
      return "questionNode";
    case MindMapNodeType.SearchQuery:
      return "searchQueryNode";
    case MindMapNodeType.SearchResult:
      return "searchResultNode";
    default:
      return "default";
  }
};

export default function ResearchMindMapFlow({
  mindMapData,
}: ResearchMindMapFlowProps) {
  // Transform the hierarchical mind map data into React Flow nodes and edges
  const { nodes, edges } = useMemo(() => {
    const nodes: Node[] = [];
    const edges: Edge[] = [];
    const processedNodeIds = new Set<string>();

    // Helper function to add a node and its edge to the parent
    const addNodeAndEdge = (
      currentNodeData: MindMapNode,
      position: { x: number; y: number },
      type: string,
      parentNodeId: string | null
    ) => {
      if (processedNodeIds.has(currentNodeData.id)) return;

      const flowNode: Node = {
        id: currentNodeData.id,
        type,
        position,
        data: { ...currentNodeData },
      };
      nodes.push(flowNode);
      processedNodeIds.add(currentNodeData.id);

      if (parentNodeId) {
        edges.push({
          id: createEdgeId(parentNodeId, currentNodeData.id),
          source: parentNodeId,
          target: currentNodeData.id,
          type: "smoothstep",
          animated: currentNodeData.type === MindMapNodeType.SearchQuery,
        });
      }
      return flowNode;
    };

    // Recursive function to layout nodes in a radial pattern
    const layoutChildren = (
      currentMindMapNode: MindMapNode,
      parentFlowNode: Node,
      parentAngleRelativeToGrandparent: number,
      radius: number
    ) => {
      if (
        !currentMindMapNode.children ||
        currentMindMapNode.children.length === 0
      ) {
        return;
      }

      const children = currentMindMapNode.children;
      const numChildren = children.length;

      // Determine the spread for children of this parent
      let angleSpread: number;
      switch (currentMindMapNode.type) {
        case MindMapNodeType.Root:
          angleSpread = 2 * Math.PI; // Questions spread in a full circle
          break;
        case MindMapNodeType.Question:
          angleSpread = Math.PI; // Queries spread over 180 degrees
          break;
        case MindMapNodeType.SearchQuery:
          angleSpread = Math.PI / 1.5; // Results spread over 120 degrees
          break;
        default:
          angleSpread = Math.PI;
      }

      children.forEach((childMindMapNode, index) => {
        let relativeAngleToParent: number;

        if (angleSpread === 2 * Math.PI) {
          // Full circle distribution
          relativeAngleToParent = (index / numChildren) * 2 * Math.PI;
        } else {
          // Arc distribution
          if (numChildren === 1) {
            relativeAngleToParent = 0; // Center of the arc
          } else {
            relativeAngleToParent =
              -angleSpread / 2 + (index / (numChildren - 1)) * angleSpread;
          }
        }

        // The child's absolute angle in the coordinate system
        const childAbsoluteAngle =
          parentAngleRelativeToGrandparent + relativeAngleToParent;

        const childX =
          parentFlowNode.position.x + radius * Math.cos(childAbsoluteAngle);
        const childY =
          parentFlowNode.position.y + radius * Math.sin(childAbsoluteAngle);

        const childFlowNode = addNodeAndEdge(
          childMindMapNode,
          { x: childX, y: childY },
          getNodeType(childMindMapNode.type),
          parentFlowNode.id
        );

        if (childFlowNode) {
          let nextRadius: number;
          switch (childMindMapNode.type) {
            case MindMapNodeType.Question:
              nextRadius = R_QUERIES;
              layoutChildren(
                childMindMapNode,
                childFlowNode,
                childAbsoluteAngle,
                nextRadius
              );
              break;
            case MindMapNodeType.SearchQuery:
              nextRadius = R_RESULTS;
              layoutChildren(
                childMindMapNode,
                childFlowNode,
                childAbsoluteAngle,
                nextRadius
              );
              break;
          }
        }
      });
    };

    // Start with the root node at the center
    const rootFlowNode = addNodeAndEdge(
      mindMapData,
      { x: 0, y: 0 },
      getNodeType(mindMapData.type),
      null
    );

    if (rootFlowNode) {
      layoutChildren(mindMapData, rootFlowNode, 0, R_QUESTIONS);
    }

    return { nodes, edges };
  }, [mindMapData]);

  return (
    <div className="w-full h-[600px]">
      <ReactFlowProvider>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          nodeTypes={nodeTypes}
          fitView
          className="bg-gray-50"
          minZoom={0.2}
          maxZoom={1.5}
          defaultViewport={{ x: 0, y: 0, zoom: 0.8 }}
        >
          <Controls />
          <Background color="#aaa" gap={16} />
        </ReactFlow>
      </ReactFlowProvider>
    </div>
  );
}
