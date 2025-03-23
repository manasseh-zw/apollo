import {
  Card,
  CardHeader,
  Button,
  CardBody,
  Chip,
  CardFooter,
  Progress,
  Image,
} from "@heroui/react";
import { MoreHorizontal } from "lucide-react";
import type { Project } from "../../lib/types/research";

export default function ProjectCard({ project }: { project: Project }) {
  const {
    id,
    title,
    image,
    status,
    description,
    startDate,
    expectedEnd,
    progress,
    endDate,
  } = project;

  const taskStats = calculateTaskStats(progress);

  return (
    <Card className="w-full overflow-hidden" shadow="sm" radius="sm">
      <CardHeader className="flex gap-3 px-5 py-4">
        <Image
          alt={title}
          height={40}
          radius="sm"
          src={image || "/placeholder.svg"}
          width={40}
          className="object-cover"
        />
        <div className="flex flex-col flex-1">
          <p className="text-small text-gray-800">{title}</p>
        </div>
        <Button isIconOnly size="sm" variant="light">
          <MoreHorizontal size={20} />
        </Button>
      </CardHeader>

      <hr />

      <div className="grid grid-cols-3 divide-x">
        <div className="p-4 text-center">
          <p className="text-xl font-light">{taskStats.total}</p>
          <p className="text-small text-gray-600`">Sources </p>
        </div>
        <div className="p-4 text-center">
          <p className="text-xl font-light text-primary">
            {taskStats.inProgress}
          </p>
          <p className="text-small text-gray-600`">Crawled</p>
        </div>
        <div className="p-4 text-center">
          <p className="text-xl font-light text-success">
            {taskStats.completed}
          </p>
          <p className="text-small text-gray-600`">Analysed</p>
        </div>
      </div>

      <hr />

      <CardBody className="px-5 py-4 space-y-3">
        <div className="flex justify-between items-center">
          <span className="text-small text-gray-600`">Due date</span>
          <span className="text-small font-medium">{expectedEnd}</span>
        </div>

        <div className="flex justify-between items-center">
          <span className="text-small text-gray-600`">Status</span>
          <Chip
            color={getStatusColor(status)}
            className="border-1 border-success"
            variant="dot"
            size="sm"
          >
            {formatStatus(status)}
          </Chip>
        </div>

        <div className="flex justify-between items-center">
          <span className="text-small text-gray-600`">Start date</span>
          <span className="text-small font-medium">{startDate}</span>
        </div>
      </CardBody>
      <hr />
      <CardFooter className="px-5 py-4 flex items-center gap-4">
        <div className="flex-1">
          <Progress
            value={progress}
            color={progress === 100 ? "success" : "primary"}
            size="sm"
            className="max-w-full"
          />
          <div className="flex justify-end mt-1">
            <span className="text-tiny text-gray-600`">{progress}%</span>
          </div>
        </div>
      </CardFooter>
    </Card>
  );
}

// Helper function to get status color
const getStatusColor = (status: string) => {
  switch (status.toLowerCase()) {
    case "complete":
    case "completed":
      return "success";
    case "in-progress":
      return "secondary";
    case "planning":
      return "secondary";
    default:
      return "default";
  }
};

// Helper function to format status display
const formatStatus = (status: string) => {
  switch (status.toLowerCase()) {
    case "in-progress":
      return "In Progress";
    default:
      return status.charAt(0).toUpperCase() + status.slice(1);
  }
};

// Calculate task statistics based on progress
const calculateTaskStats = (progress: number) => {
  const totalTasks = Math.floor(Math.random() * 50) + 20;
  const completedTasks = Math.floor(totalTasks * (progress / 100));
  const inProgressTasks = Math.floor((totalTasks - completedTasks) / 2);

  return {
    total: totalTasks,
    completed: completedTasks,
    inProgress: inProgressTasks,
  };
};
