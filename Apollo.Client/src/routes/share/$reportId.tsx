import { createFileRoute } from "@tanstack/react-router";
import ResearchReport from "../../components/research/report/ResearchReport";
import { getSharedResearchReport } from "../../lib/services/research.service";

export const Route = createFileRoute("/share/$reportId")({
  component: SharedResearchReport,
  loader: async ({ params }) => {
    const result = await getSharedResearchReport(params.reportId);

    if (!result.success) {
      throw new Error("Research report not found");
    }

    return {
      report: {
        id: result.data.id,
        title: result.data.title,
        content: result.data.content,
      },
    };
  },
});

function SharedResearchReport() {
  const { report } = Route.useLoaderData();
  return <ResearchReport report={report} isSharedView />;
}