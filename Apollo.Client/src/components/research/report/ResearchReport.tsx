import { useState } from "react";
import {
  Button,
  ButtonGroup,
  Dropdown,
  DropdownTrigger,
  DropdownMenu,
  DropdownItem,
} from "@heroui/react"; // Assuming this path is correct
import { ChevronDownIcon } from "lucide-react";
import { MSWord, MSPowerPoint, PDFIcon } from "../../Icons"; // Assuming this path is correct
import FontSizeController from "./FontSizeController"; // Assuming this path is correct
import type { ResearchReport as ResearchReportType } from "../../../lib/types/research"; // Assuming this path is correct
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";

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

      {/* Main Content */}
      {/* flex-1: Allows this section to grow/shrink vertically to fill available space */}
      <main className="flex-1 overflow-auto p-8 md:px-16 lg:px-24 pb-24 font-geist">
        {/* max-w-3xl mx-auto: Limits content width and centers it */}
        {/* prose...: Applies Tailwind Typography styles */}
        <div
          className="max-w-3xl mx-auto prose prose-slate text-primary prose-li:text-primary prose-p:text-primary prose-headings:text-primary prose-strong:text-foreground prose-code:text-foreground prose-pre:bg-content2 prose-pre:text-foreground prose-pre:border prose-pre:border-content4 prose-pre:rounded-lg prose-a:text-primary prose-p:leading-relaxed prose-li:leading-relaxed [&>*]:leading-relaxed [&>ul]:list-none [&>ul>li]:relative [&>ul>li]:before:absolute [&>ul>li]:before:left-0 [&>ul>li]:before:content-['•'] [&>ul>li]:before:text-primary-400 prose-hr:border-content4 prose-hr:my-5 break-words" 
          style={{ fontSize: `${fontSize}px` }}
        >
          <ReactMarkdown remarkPlugins={[remarkGfm]}>
            {report.content}
          </ReactMarkdown>
        </div>
      </main>

      {/* Font Size Controller */}
      {/* Relative positioning might be fine, depends on FontSizeController's implementation */}
      <div className="relative">
        <FontSizeController
          fontSize={fontSize}
          onFontSizeChange={setFontSize}
        />
      </div>
    </div>
  );
}