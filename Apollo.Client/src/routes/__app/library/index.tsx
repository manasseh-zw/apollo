import { createFileRoute } from "@tanstack/react-router";

import { Input } from "@heroui/react";
import { Search } from "lucide-react";
import { protectedLoader } from "../../../lib/utils/loaders";
import type { Project } from "../../../lib/types/research";
import BentoGrid from "../../../components/BentoGrid";
import ProjectCard from "../../../components/library/ProjectCard";

export const Route = createFileRoute("/__app/library/")({
  component: Library,
  loader: protectedLoader,
});

// Enhanced sample data with all required fields
const projects: Project[] = [
  {
    id: 1,
    title: "AI-Powered Climate Change Prediction",
    image: "https://picsum.photos/seed/ai-climate/400/300",
    status: "in-progress",
    description:
      "Using machine learning to predict climate change patterns and impacts",
    startDate: "Jan 2024",
    expectedEnd: "Dec 2024",
    progress: 45,
    endDate: "",
  },
  {
    id: 2,
    title: "Quantum Computing Applications in Cryptography",
    image: "https://picsum.photos/seed/quantum/400/300",
    status: "in-progress",
    description:
      "Exploring quantum algorithms for next-generation cryptographic systems",
    startDate: "Mar 2024",
    expectedEnd: "Mar 2025",
    progress: 30,
    endDate: "",
  },
  {
    id: 3,
    title: "Novel Drug Delivery Systems",
    image: "https://picsum.photos/seed/drug/400/300",
    status: "complete",
    description:
      "Developing targeted drug delivery mechanisms for improved efficacy",
    startDate: "Mar 2023",
    expectedEnd: "Mar 2024",
    progress: 100,
    endDate: "Mar 2024",
  },
];

function Library() {
  return (
    <div className="w-full  flex-1 p-6 ">
      {projects.length > 0 ? (
        <div className="w-full flex-1 p-6 md:px-12 lg:px-16">
          <div className="flex items-center gap-x-3 mb-4 justify-center md:justify-start">
            <h1 className="text-2xl font-light leading-9 text-default-foreground">
              Research Projects
            </h1>
          </div>

          <div className="flex flex-wrap items-end justify-between gap-4 ">
            <div className="max-sm:w-full sm:flex-1">
              <div className="mt-4 flex max-w-sm gap-4">
                <Input
                  color="primary"
                  variant="bordered"
                  startContent={<Search />}
                  placeholder="Search projects..."
                  className="flex-1"
                />
              </div>
            </div>
            {/* {asside} */}
          </div>

          <div className="mt-10 grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6 ">
            {projects.map((project) => (
              <ProjectCard key={project.id} project={project} />
            ))}
          </div>
        </div>
      ) : (
        <>
          <div className="flex items-center gap-x-3 mb-4 justify-center md:justify-start">
            <h1 className="text-2xl font-light leading-9 text-default-foreground">
              Library
            </h1>
          </div>
          <h2 className="text-small text-secondary-700 mb-6">
            Good morning what would you like to research on today?
          </h2>
          <BentoGrid />
        </>
      )}
    </div>
  );
}
