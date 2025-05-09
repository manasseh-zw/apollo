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
import Html from "react-pdf-html";

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
});

// HTML styles for react-pdf-html
const htmlStyles = {
  body: {
    paddingBottom: 20,
    fontSize: 12,
    lineHeight: 1.5,
    fontFamily: "Times-Roman",
  },
  p: {
    marginBottom: 10,
    textAlign: "justify",
  },
  h1: {
    fontSize: 20,
    fontWeight: "bold",
    marginBottom: 10,
  },
  h2: {
    fontSize: 18,
    fontWeight: "bold",
    marginBottom: 8,
  },
  h3: {
    fontSize: 16,
    fontWeight: "bold",
    marginBottom: 6,
  },
  ul: {
    marginBottom: 10,
  },
  ol: {
    marginBottom: 10,
  },
  li: {
    marginBottom: 4,
    marginLeft: 15,
  },
  a: {
    color: "blue",
    textDecoration: "underline",
  },
  strong: {
    fontWeight: "bold",
  },
  em: {
    fontStyle: "italic",
  },
  blockquote: {
    marginLeft: 20,
    paddingLeft: 10,
    borderLeft: "2px solid gray",
    fontStyle: "italic",
  },
  code: {
    fontFamily: "Courier",
    backgroundColor: "#f0f0f0",
    padding: "2px 4px",
  },
  pre: {
    fontFamily: "Courier",
    backgroundColor: "#f0f0f0",
    padding: 10,
    marginBottom: 10,
  },
  table: {
    width: "100%",
    marginBottom: 10,
    borderCollapse: "collapse",
  },
  th: {
    borderBottom: "1px solid black",
    padding: 5,
    fontWeight: "bold",
  },
  td: {
    borderBottom: "1px solid #ddd",
    padding: 5,
  },
};

// PDF Document Component
const ReportPDFDocument = ({
  title,
  content,
  htmlContent,
}: {
  title: string;
  content: string;
  htmlContent: string;
}) => (
  <Document>
    <Page size="A4" style={styles.page}>
      <View>
        <Text style={styles.title}>{title}</Text>
        <Html stylesheet={htmlStyles}>{htmlContent}</Html>
      </View>
    </Page>
  </Document>
);

interface PrintReportModalProps {
  isOpen: boolean;
  onClose: () => void;
  reportTitle: string;
  reportContent: string;
  reportHtmlContent: string;
}

export default function PrintReportModal({
  isOpen,
  onClose,
  reportTitle,
  reportContent,
  reportHtmlContent,
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
                  htmlContent={reportHtmlContent}
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
              <ReportPDFDocument
                title={reportTitle}
                content={reportContent}
                htmlContent={reportHtmlContent}
              />
            </PDFViewer>
          </div>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
