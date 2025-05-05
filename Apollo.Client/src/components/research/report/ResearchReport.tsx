import { useRef, useState } from "react";
import {
  Button,
  Dropdown,
  DropdownTrigger,
  DropdownMenu,
  DropdownItem,
  Modal,
  ModalContent,
  ModalBody,
  Popover,
  PopoverTrigger,
  PopoverContent,
  Slider,
} from "@heroui/react";
import { Share2, Download, Type } from "lucide-react";
import { MSWord, MSPowerPoint, PDFIcon } from "../../Icons";
import type { ResearchReport as ResearchReportType } from "../../../lib/types/research";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import { useReactToPrint } from "react-to-print";
import ShareModal from "./ShareModal";
import Banner from "./Banner";

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
    <div className="flex flex-col h-full bg-white relative">
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

      {isSharedView && <Banner />}
      <main className="flex overflow-y-auto overflow-x-hidden justify-center py-10">
        <div className="max-w-3xl w-full font-geist px-4">
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

      {/* Floating Action Buttons */}
      <div className="absolute bottom-20 lg:right-48 md:right-12 right-2 flex flex-col gap-2">
        {/* Font Size Button */}
        <Popover placement="left" showArrow offset={10}>
          <PopoverTrigger>
            <Button
              isIconOnly
              variant="flat"
              radius="full"
              className="bg-primary text-primary-foreground"
            >
              <Type size={20} />
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-[240px] p-4">
            {(titleProps) => (
              <div className="w-full">
                <p
                  className="text-sm font-medium text-foreground mb-2"
                  {...titleProps}
                >
                  Font Size
                </p>
                <div className="flex items-center gap-4">
                  <Slider
                    size="sm"
                    color="primary"
                    value={fontSize}
                    minValue={12}
                    maxValue={24}
                    step={1}
                    onChange={(value) => setFontSize(value as number)}
                    aria-label="Font size"
                    className="flex-1"
                  />
                  <span className="text-sm font-medium min-w-[40px] text-right">
                    {fontSize}px
                  </span>
                </div>
              </div>
            )}
          </PopoverContent>
        </Popover>

        {/* Share Button - Only show if not in shared view */}
        {!isSharedView && (
          <Button
            isIconOnly
            variant="flat"
            radius="full"
            className="bg-primary text-primary-foreground"
            onPress={() => setIsShareModalOpen(true)}
          >
            <Share2 size={20} />
          </Button>
        )}

        {/* Export Button */}
        <Dropdown placement="left">
          <DropdownTrigger>
            <Button
              isIconOnly
              variant="flat"
              radius="full"
              className="bg-primary text-primary-foreground"
              onPress={handleExportClick}
            >
              <Download size={20} />
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
      </div>
    </div>
  );
}
