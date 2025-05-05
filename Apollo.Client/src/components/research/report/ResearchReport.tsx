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
import { ChevronDownIcon, Share2 } from "lucide-react";
import { MSWord, MSPowerPoint, PDFIcon } from "../../Icons";
import FontSizeController from "./FontSizeController";
import type { ResearchReport as ResearchReportType } from "../../../lib/types/research";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import { useReactToPrint } from "react-to-print";
import ShareModal from "./ShareModal";

interface ResearchReportProps {
  report: ResearchReportType | null;
  isSharedView?: boolean;
}

export default function ResearchReport({
  report,
  isSharedView = false,
}: ResearchReportProps) {
  const [fontSize, setFontSize] = useState(16);
  const [selectedExportOption, setSelectedExportOption] = useState(
    new Set(["pdf"])
  );
  const [isShareModalOpen, setIsShareModalOpen] = useState(false);
  const [isExportModalOpen, setIsExportModalOpen] = useState(false);

  const reportContentRef = useRef<HTMLDivElement>(null);

  const exportOptions = {
    pdf: { text: "Export as PDF", icon: PDFIcon },
    word: { text: "Export as Word", icon: MSWord },
    powerpoint: { text: "Export as PowerPoint", icon: MSPowerPoint },
  };

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

  const handlePrint = useReactToPrint({
    contentRef: reportContentRef,
    documentTitle: report.title,
    pageStyle: `
        @page {
          size: A4;
          margin: 20mm;
        }
        @media print {
          body {
            -webkit-print-color-adjust: exact;
            print-color-adjust: exact;
          }
          .prose {
            max-width: none !important;
          }
          main {
             overflow: visible !important;
             display: block !important;
          }
        }
      `,
  });

  const handleExportClick = () => {
    if (selectedOption === "pdf") {
      handlePrint();
    } else {
      setIsExportModalOpen(true);
    }
  };

  return (
    <div className="flex flex-col h-full bg-white">
      <Modal
        size="sm"
        isOpen={isExportModalOpen}
        onClose={() => setIsExportModalOpen(false)}
      >
        <ModalContent className="p-3">
          <ModalBody>
            <p className="text-xl font-bold">
              Sorry this is still in construction, coming soon though! ☺️
            </p>
          </ModalBody>
        </ModalContent>
      </Modal>

      <ShareModal
        isOpen={isShareModalOpen}
        onClose={() => setIsShareModalOpen(false)}
        reportId={report.id}
      />

      {/* Top Header */}
      <header className="py-4 px-4 sm:px-6 md:px-8">
        <div className="max-w-3xl mx-auto flex justify-end gap-2">
          {!isSharedView && (
            <Button
              variant="flat"
              size="sm"
              startContent={<Share2 size={18} />}
              onPress={() => setIsShareModalOpen(true)}
            >
              Share
            </Button>
          )}
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
      <main className="flex overflow-y-auto overflow-x-hidden justify-center">
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
