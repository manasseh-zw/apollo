import { createFileRoute, redirect } from "@tanstack/react-router";
import AgentChat from "../../../../components/research/AgentChat";
import ResearchFeed from "../../../../components/research/feed/ResearchFeed";
import ResearchReport from "../../../../components/research/report/ResearchReport";
import { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import { config } from "../../../../../config";
import {
  type ResearchFeedUpdate,
  type AgentChatMessage,
  type ResearchResponse,
  type ResearchUpdatesResponse,
  ResearchStatus,
  type TimelineUpdate,
  type ResearchReport as ResearchReportType,
} from "../../../../lib/types/research";
import {
  getResearchById,
  getResearchUpdates,
} from "../../../../lib/services/research.service";
import { protectedLoader } from "../../../../lib/utils/loaders";
import { LogoLight } from "../../../../components/Icons";

function LoadingState() {
  return (
    <div className="flex flex-col items-center justify-center h-full w-full gap-4">
      <div className="animate-pulse bg-primary p-[.55rem] rounded-full">
        <LogoLight width={32} height={32} />
      </div>
    </div>
  );
}

export const Route = createFileRoute("/__app/research/$id/")({
  component: RouteComponent,
  pendingComponent: LoadingState,
  loader: async ({ params }) => {
    // First run the protected route loader
    await protectedLoader();

    // Then fetch the research data
    const result = await getResearchById(params.id);

    if (!result.success) {
      throw redirect({
        to: "/research",
        search: {
          error: "Research not found",
        },
      });
    }

    const research = result.data as ResearchResponse;

    // If research is in progress, fetch cached updates
    let updates: ResearchUpdatesResponse | null = null;
    if (research.status === ResearchStatus.InProgress) {
      const updatesResult = await getResearchUpdates(params.id);
      console.log(updatesResult);
      if (updatesResult.success) {
        updates = updatesResult.data;
      }
    }

    return {
      research,
      updates,
    };
  },
});

function RouteComponent() {
  const { research: initialResearch, updates } = Route.useLoaderData();
  const [research, setResearch] = useState<ResearchResponse>(initialResearch);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );
  const { id: researchId } = Route.useParams();

  useEffect(() => {
    // Reset research state when ID changes
    setResearch(initialResearch);
  }, [initialResearch]);

  useEffect(() => {
    // Only set up SignalR if research is in progress
    if (research.status === ResearchStatus.Complete) {
      return;
    }

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${config.url}/hubs/research`)
      .withAutomaticReconnect()
      .build();

    // Set up event handlers
    newConnection.on("ReceiveTimelineUpdate", (update: TimelineUpdate) => {
      console.log("Timeline update:", update);
    });

    newConnection.on(
      "ReceiveResearchFeedUpdate",
      (update: ResearchFeedUpdate) => {
        console.log("Feed update:", update);
      }
    );

    newConnection.on("ReceiveAgentChatMessage", (message: AgentChatMessage) => {
      console.log("Agent message:", message);
    });

    newConnection.on(
      "ResearchCompletedWithReport",
      (researchId: string, report: ResearchReportType) => {
        console.log("Research completed with report:", researchId, report);
        setResearch((prev) => ({
          ...prev,
          status: ResearchStatus.Complete,
          report,
        }));
      }
    );

    // Start the connection
    newConnection
      .start()
      .then(() => {
        setConnection(newConnection);
        // Join the research-specific group
        return newConnection.invoke("JoinResearchStream", researchId);
      })
      .catch((err) => console.error("SignalR Connection Error: ", err));

    // Cleanup on unmount
    return () => {
      if (newConnection) {
        newConnection
          .invoke("LeaveResearchStream", researchId)
          .catch(console.error);
        newConnection.stop();
      }
    };
  }, [researchId, research.status]);

  // Use key prop to force remount when research ID changes
  const content =
    research.status === ResearchStatus.Complete ? (
      <ResearchReport key={researchId} report={research.report} />
    ) : (
      <main
        key={researchId}
        className="w-full h-full grid grid-cols-12 gap-4 bg-white p-1"
      >
        <div className="h-full col-span-8 border-r border-gray-200">
          <ResearchFeed
            connection={connection}
            researchId={researchId}
            research={research}
            initialFeedUpdates={updates?.feedUpdates}
          />
        </div>
        <div className="h-full col-span-4 flex flex-col overflow-hidden py-2">
          <AgentChat
            connection={connection}
            researchId={researchId}
            initialChatMessages={updates?.chatMessages}
          />
        </div>
      </main>
    );

  return content;
}
