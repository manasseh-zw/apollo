import {
  ReactFlowProvider,
  Controls,
  Background,
  type Node,
  type Edge,
  Position,
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

    // Function to process a node and its children
    const processNode = (
      node: MindMapNode,
      depth: number = 0,
      index: number = 0,
      parentId: string | null = null
    ) => {
      // Calculate position based on depth and index
      const xGap = 250; // Horizontal spacing between levels
      const yGap = 150; // Vertical spacing between siblings
      const x = depth * xGap;
      const y = index * yGap;

      // Create the node
      const flowNode: Node = {
        id: node.id,
        type: getNodeType(node.type),
        position: { x, y },
        data: { ...node }, // Pass all node data
        // Default sourcePosition and targetPosition for horizontal tree layout
        sourcePosition: Position.Right,
        targetPosition: Position.Left,
      };

      // Add node to the collection
      nodes.push(flowNode);

      // If this node has a parent, create an edge
      if (parentId) {
        edges.push({
          id: createEdgeId(parentId, node.id),
          source: parentId,
          target: node.id,
          type: "smoothstep", // Smooth curved edges
          animated: node.type === MindMapNodeType.SearchQuery, // Animate edges to search queries
        });
      }

      // Process children
      if (node.children) {
        node.children.forEach((child, childIndex) => {
          processNode(child, depth + 1, index + childIndex, node.id);
        });
      }
    };

    // Start processing from the root
    processNode(mindMapData);

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
