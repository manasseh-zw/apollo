import { createFileRoute } from "@tanstack/react-router";
import ResearchReport from "../../components/research/report/ResearchReport";
import { getSharedResearchReport } from "../../lib/services/research.service";

// Define the report type for type safety
interface Report {
  id: string;
  title: string;
  content: string;
}

// Define the loader return type
interface LoaderData {
  report: Report;
}

export const Route = createFileRoute("/share/$reportId")({
  component: SharedResearchReport,
  loader: async ({ params }): Promise<LoaderData> => {
    const result = await getSharedResearchReport(params.reportId);

    if (!result.success) {
      throw new Error("Research report not found");
    }

    return {
      report: {
        //@ts-ignore
        id: result.data.id,
        //@ts-ignore
        title: result.data.title,
        //@ts-ignore
        content: result.data.content,
      },
    };
  },
});

function SharedResearchReport() {
  const { report } = Route.useLoaderData();
  return <ResearchReport report={report} isSharedView />;
}
