import { Modal, ModalContent, ModalBody, Button } from "@heroui/react";
import { Download } from "lucide-react";
import {
  Document,
  Page,
  Text,
  View,
  StyleSheet,
  PDFViewer,
  PDFDownloadLink,
} from "@react-pdf/renderer";

// Define styles for PDF document
const styles = StyleSheet.create({
  page: {
    flexDirection: "column",
    backgroundColor: "#ffffff",
    padding: 40,
  },
  title: {
    fontSize: 24,
    marginBottom: 20,
    fontWeight: "bold",
  },
  content: {
    fontSize: 12,
    lineHeight: 1.5,
    textAlign: "justify",
  },
});

// PDF Document Component
const ReportPDFDocument = ({
  title,
  content,
}: {
  title: string;
  content: string;
}) => (
  <Document>
    <Page size="A4" style={styles.page}>
      <View>
        <Text style={styles.title}>{title}</Text>
        <Text style={styles.content}>{content}</Text>
      </View>
    </Page>
  </Document>
);

interface PrintReportModalProps {
  isOpen: boolean;
  onClose: () => void;
  reportTitle: string;
  reportContent: string;
}

export default function PrintReportModal({
  isOpen,
  onClose,
  reportTitle,
  reportContent,
}: PrintReportModalProps) {
  return (
    <Modal
      size="3xl"
      isOpen={isOpen}
      onClose={onClose}
      scrollBehavior="inside"
      classNames={{
        base: "h-[90vh]",
      }}
    >
      <ModalContent>
        <ModalBody className="flex flex-col gap-4 p-6">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-medium">Preview PDF</h3>
            <PDFDownloadLink
              document={
                <ReportPDFDocument
                  title={reportTitle}
                  content={reportContent}
                />
              }
              fileName={`${reportTitle.toLowerCase().replace(/\s+/g, "-")}.pdf`}
            >
              {({ loading }) => (
                <Button
                  color="primary"
                  startContent={<Download size={18} />}
                  isLoading={loading}
                >
                  Download PDF
                </Button>
              )}
            </PDFDownloadLink>
          </div>

          <div className="flex-1 min-h-[60vh] border rounded-lg overflow-hidden">
            <PDFViewer
              style={{
                width: "100%",
                height: "100%",
                border: "none",
              }}
            >
              <ReportPDFDocument title={reportTitle} content={reportContent} />
            </PDFViewer>
          </div>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
