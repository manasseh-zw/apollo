import React from "react";
import {
  Button,
  Divider,
  Listbox,
  ListboxItem,
  Modal,
  ModalBody,
  ModalContent,
  ModalHeader,
} from "@heroui/react";
import type { ResearchHistoryItem } from "../../lib/types/research";
import { Link } from "@tanstack/react-router";

interface HistoryModalProps {
  items: ResearchHistoryItem[];
  isOpen: boolean;
  onOpenChange: (isOpen: boolean) => void;
}

export default function HistoryModalTrigger({
  items,
  isOpen,
  onOpenChange,
}: HistoryModalProps) {
  return (
    <Modal
      isOpen={isOpen}
      onOpenChange={onOpenChange}
      backdrop="opaque"
      size="xl"
      classNames={{}}
    >
      <ModalContent className="p-5">
        {(onClose) => (
          <div className="">
            <ModalHeader className="flex flex-col gap-1">
              <h2 className="text-xl font-semibold">Research History</h2>
            </ModalHeader>
            <Divider />
            <ModalBody className="p-0">
              <HistoryModalContent items={items} onClose={onClose} />
            </ModalBody>
          </div>
        )}
      </ModalContent>
    </Modal>
  );
}

function HistoryModalContent({
  items,
  onClose,
}: {
  items: ResearchHistoryItem[];
  onClose: () => void;
}) {
  const renderChatItem = (item: ResearchHistoryItem) => (
    <ListboxItem key={item.id} className="py-3 px-4" textValue={item.title}>
      <Link
        to="/research/$id"
        params={{ id: item.id }}
        onClick={onClose}
        className="flex justify-between items-center w-full"
      >
        <span className="text-small">{item.title}</span>
        <span className="text-small text-foreground">
          {new Date(item.startedAt).toLocaleDateString()}
        </span>
      </Link>
    </ListboxItem>
  );

  return (
    <div className="w-full  ">
      <Listbox aria-label="Research history" variant="flat" color="primary">
        {items.map(renderChatItem)}
      </Listbox>
    </div>
  );
}
