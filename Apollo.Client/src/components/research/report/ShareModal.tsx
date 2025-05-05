import { useState } from "react";
import { Modal, ModalContent, ModalBody, Button, Input } from "@heroui/react";
import { Copy, Check } from "lucide-react";

interface ShareModalProps {
  isOpen: boolean;
  onClose: () => void;
  reportId: string;
}

export default function ShareModal({
  isOpen,
  onClose,
  reportId,
}: ShareModalProps) {
  const [copied, setCopied] = useState(false);
  const shareUrl = `${window.location.origin}/share/${reportId}`;

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(shareUrl);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      console.error("Failed to copy:", err);
    }
  };

  return (
    <Modal size="md" isOpen={isOpen} onClose={onClose}>
      <ModalContent className="p-6">
        <ModalBody>
          <h3 className="text-lg font-medium mb-4">Share Research Report</h3>
          <p className="text-sm text-gray-600 mb-4">
            Anyone with this link can view the research report:
          </p>
          <div className="flex gap-2">
            <Input
              value={shareUrl}
              readOnly
              className="flex-1"
              classNames={{
                input: "bg-gray-50",
              }}
            />
            <Button
              color={copied ? "success" : "primary"}
              onPress={handleCopy}
              startContent={copied ? <Check size={18} /> : <Copy size={18} />}
            >
              {copied ? "Copied!" : "Copy"}
            </Button>
          </div>
        </ModalBody>
      </ModalContent>
    </Modal>
  );
}
