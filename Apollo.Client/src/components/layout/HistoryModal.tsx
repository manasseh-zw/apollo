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

export default function HistoryModalTrigger(items: ResearchHistoryItem[]) {
  return (
    <div>
      <Button onPress={onOpen} color="primary" variant="solid">
        History
      </Button>

      <Modal
        isOpen={isOpen}
        onOpenChange={onOpenChange}
        backdrop="blur"
        classNames={{
          backdrop: "bg-blue-600/50",
          base: "bg-content1",
        }}
      >
        <ModalContent>
          {(onClose) => (
            <>
              <ModalHeader className="flex flex-col gap-1">
                <h2 className="text-xl font-semibold">Chat History</h2>
              </ModalHeader>
              <Divider />
              <ModalBody className="p-0">
                <HistoryModalContent items={items} />
              </ModalBody>
            </>
          )}
        </ModalContent>
      </Modal>
    </div>
  );
}

function HistoryModalContent({
  items,
}: {
  items: ResearchHistoryItem[];
}) {
  const renderChatItem = (chat: {
    id: string;
    title: string;
    date: string;
  }) => (
    <ListboxItem key={chat.id} className="py-3 px-4" textValue={chat.title}>
      <div className="flex justify-between items-center w-full">
        <span className="text-medium">{chat.title}</span>
        <span className="text-small text-default-500">{chat.date}</span>
      </div>
    </ListboxItem>
  );

  return (
    <div className="w-full">
      <Listbox aria-label="Chat history" variant="flat" color="default">
        {items.map(renderChatItem)}
      </Listbox>
    </div>
  );
}
