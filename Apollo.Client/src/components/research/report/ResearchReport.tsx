import { useRef, useState } from "react";
import {
  Button,
  Popover,
  PopoverTrigger,
  PopoverContent,
  Slider,
} from "@heroui/react";
import { Share2, Download, Type } from "lucide-react";
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
  const [isShareModalOpen, setIsShareModalOpen] = useState(false);

  const reportContentRef = useRef<HTMLDivElement>(null);

  if (!report) {
    return (
      <div className="flex items-center justify-center h-full">
        <p className="text-gray-500">No report available yet.</p>
      </div>
    );
  }

  const handlePrint = useReactToPrint({
    contentRef: reportContentRef, // Ensure content returns the ref's current node
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
    handlePrint();
  };

  return (
    <div className="flex flex-col h-full bg-white relative">
      <ShareModal
        isOpen={isShareModalOpen}
        onClose={() => setIsShareModalOpen(false)}
        reportId={report.id}
      />

      {isSharedView && <Banner />}
      <main className="flex overflow-y-auto overflow-x-hidden justify-center py-10">
        <div className="max-w-3xl w-full font-geist px-4">
          {/* This div contains the content to be printed */}
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
      <div className="absolute bottom-24 lg:right-[14rem] md:right-12 right-2 flex flex-col gap-2">
        {/* Font Size Button (Unchanged) */}
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

        {/* Share Button - Only show if not in shared view (Unchanged) */}
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

        <Button
          isIconOnly
          variant="flat"
          radius="full"
          className="bg-primary text-primary-foreground"
          onPress={handleExportClick}
          aria-label="Export as PDF"
        >
          <Download size={20} />
        </Button>
      </div>
    </div>
  );
}
