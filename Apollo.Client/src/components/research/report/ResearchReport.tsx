import { useState } from "react";
import {
  Button,
  ButtonGroup,
  Dropdown,
  DropdownTrigger,
  DropdownMenu,
  DropdownItem,
} from "@heroui/react";
import { ChevronDownIcon } from "lucide-react";
import { MSWord, MSPowerPoint, PDFIcon } from "../../Icons";
import FontSizeController from "./FontSizeController";
import type { ResearchReport as ResearchReportType } from "../../../lib/types/research";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import rehypeRaw from "rehype-raw";

interface ResearchReportProps {
  report: ResearchReportType | null;
}

export default function ResearchReport({ report }: ResearchReportProps) {
  const [fontSize, setFontSize] = useState(16);
  const [selectedExportOption, setSelectedExportOption] = useState(
    new Set(["pdf"])
  );

  const exportOptions = {
    pdf: { text: "Export as PDF", icon: PDFIcon },
    word: { text: "Export as Word", icon: MSWord },
    powerpoint: { text: "Export as PowerPoint", icon: MSPowerPoint },
  };

  // Get the currently selected export option
  const selectedOption = Array.from(
    selectedExportOption
  )[0] as keyof typeof exportOptions;

  if (!report) {
    return (
      <div className="flex items-center justify-center h-full">
        <p className="text-gray-500">No report available yet.</p>
      </div>
    );
  }

  return (
    // flex flex-col h-full: Ensures the component takes full height and lays out children vertically
    <div className="flex flex-col h-full bg-white">
      {/* Top Header */}
      {/* Added some padding for consistency */}
      <header className="pt-4 px-4 sm:px-6 md:px-8">
        <div className="max-w-3xl mx-auto flex justify-end">
          <ButtonGroup variant="flat">
            <Button className="bg-primary text-primary-foreground">
              {exportOptions[selectedOption]?.text || "Export"}
            </Button>
            <Dropdown placement="bottom-end">
              <DropdownTrigger>
                <Button
                  isIconOnly
                  className="bg-primary text-primary-foreground hover:bg-primary/95"
                >
                  <ChevronDownIcon />
                </Button>
              </DropdownTrigger>
              <DropdownMenu
                disallowEmptySelection
                aria-label="Export options"
                selectedKeys={selectedExportOption}
                color="primary"
                selectionMode="single"
                //@ts-ignore - Keep if necessary for your library version
                onSelectionChange={setSelectedExportOption}
              >
                <DropdownItem
                  key="word"
                  startContent={<MSWord className="h-5 w-5" />}
                >
                  Export as Word
                </DropdownItem>
                <DropdownItem
                  key="powerpoint"
                  startContent={<MSPowerPoint className="h-5 w-5" />}
                >
                  Export as PowerPoint
                </DropdownItem>
                <DropdownItem
                  key="pdf"
                  startContent={<PDFIcon className="h-5 w-5" />}
                >
                  Export as PDF
                </DropdownItem>
              </DropdownMenu>
            </Dropdown>
          </ButtonGroup>
        </div>
      </header>

      <main className="flex overflow-auto justify-center pb-5">
        <div className="max-w-3xl font-geist">
          <div className="markdown" style={{ fontSize: `${fontSize}px` }}>
            <ReactMarkdown remarkPlugins={[remarkGfm]}>
              {report.content}
            </ReactMarkdown>
          </div>
        </div>
      </main>

      <div className="relative">
        <FontSizeController
          fontSize={fontSize}
          onFontSizeChange={setFontSize}
        />
      </div>
    </div>
  );
}
