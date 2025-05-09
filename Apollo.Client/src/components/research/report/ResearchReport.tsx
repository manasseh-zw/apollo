import { useState, useRef } from "react";
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
import ShareModal from "./ShareModal";
import PrintReportModal from "./PrintReportModal";
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
  const [isPrintModalOpen, setIsPrintModalOpen] = useState(false);
  const [reportHtmlForPdf, setReportHtmlForPdf] = useState("");
  const markdownRef = useRef<HTMLDivElement>(null);

  if (!report) {
    return (
      <div className="flex items-center justify-center h-full">
        <p className="text-gray-500">No report available yet.</p>
      </div>
    );
  }

  const handlePrintClick = () => {
    if (markdownRef.current) {
      setReportHtmlForPdf(markdownRef.current.innerHTML);
      setIsPrintModalOpen(true);
    }
  };

  return (
    <div className="flex flex-col h-full bg-white relative">
      <ShareModal
        isOpen={isShareModalOpen}
        onClose={() => setIsShareModalOpen(false)}
        reportId={report.id}
      />

      <PrintReportModal
        isOpen={isPrintModalOpen}
        onClose={() => setIsPrintModalOpen(false)}
        reportTitle={report.title}
        reportContent={report.content}
        reportHtmlContent={reportHtmlForPdf}
      />

      {isSharedView && <Banner />}
      <main className="flex overflow-y-auto overflow-x-hidden justify-center py-10">
        <div className="max-w-3xl w-full font-geist px-4">
          <div
            ref={markdownRef}
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
      <div className="fixed bottom-20 lg:right-[14rem] md:right-12 right-2 flex flex-col gap-2">
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

        <Button
          isIconOnly
          variant="flat"
          radius="full"
          className="bg-primary text-primary-foreground"
          onPress={handlePrintClick}
          aria-label="Export as PDF"
        >
          <Download size={20} />
        </Button>
      </div>
    </div>
  );
}
