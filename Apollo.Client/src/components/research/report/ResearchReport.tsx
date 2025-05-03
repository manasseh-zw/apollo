import { useRef, useState } from "react";
import {
  Button,
  ButtonGroup,
  Dropdown,
  DropdownTrigger,
  DropdownMenu,
  DropdownItem,
  Modal,
  ModalContent,
  ModalBody,
} from "@heroui/react";
import { ChevronDownIcon } from "lucide-react";
import { MSWord, MSPowerPoint, PDFIcon } from "../../Icons";
import FontSizeController from "./FontSizeController";
import type { ResearchReport as ResearchReportType } from "../../../lib/types/research";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import { useReactToPrint } from "react-to-print";

interface ResearchReportProps {
  report: ResearchReportType | null;
}

export default function ResearchReport({ report }: ResearchReportProps) {
  const [fontSize, setFontSize] = useState(16);
  const [selectedExportOption, setSelectedExportOption] = useState(
    new Set(["pdf"])
  );

  const [isOpen, setIsOpen] = useState(false);

  const reportContentRef = useRef<HTMLDivElement>(null); // Create a ref to the content

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

  // Configure the react-to-print hook
  const handlePrint = useReactToPrint({
    contentRef: reportContentRef,
    documentTitle: "Research Report",
    pageStyle: `
        @page {
          size: A4; /* Or 'letter', etc. */
          margin: 20mm; /* Adjust margins as needed */
        }
        @media print {
          body {
            -webkit-print-color-adjust: exact; /* Ensures background colors/images print in Chrome/Safari */
            print-color-adjust: exact; /* Standard */
          }
          /* Ensure the prose styles apply correctly in print */
          .prose {
            max-width: none !important; /* Allow prose to fill the page width */
          }
          /* Add any other print-specific overrides */
          main {
             overflow: visible !important; /* Prevent content cutoff */
             display: block !important; /* Ensure main is block for printing */
          }
          .report-content-wrapper {
             /* Add specific print styles for the wrapper if needed */
          }
        }
      `,
  });

  const handleExportClick = () => {
    if (selectedOption === "pdf") {
      handlePrint();
    } else {
      setIsOpen(true);
    }
  };

  return (
    // flex flex-col h-full: Ensures the component takes full height and lays out children vertically
    <div className="flex flex-col h-full bg-white">
      <Modal
        size="sm"
        isOpen={isOpen}
        onClose={() => {
          setIsOpen(false);
        }}
      >
        <ModalContent className="p-3">
          <ModalBody>
            <p className="text-xl font-bold">
              Sorry this is still in construction, coming soon though! ☺️
            </p>
          </ModalBody>
        </ModalContent>
      </Modal>
      ;{/* Top Header */}
      <header className="py-4 px-4 sm:px-6 md:px-8">
        <div className="max-w-3xl mx-auto flex justify-end">
          <ButtonGroup variant="flat" size="sm">
            <Button
              className="bg-primary text-primary-foreground"
              onPress={handleExportClick}
            >
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
      <main className="flex overflow-y-auto overflow-x-hidden justify-center ">
        <div className="max-w-3xl w-full font-geist">
          <div
            ref={reportContentRef}
            className="prose prose-slate max-w-none pb-14"
            style={{ fontSize: `${fontSize}px` }}
          >
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
