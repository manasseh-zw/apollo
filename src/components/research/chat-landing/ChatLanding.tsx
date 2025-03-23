import { Image } from "@heroui/react";
import PromptForm from "./prompt-form";

export default function ChatLanding() {
  return (
    <div className="flex flex-col items-center w-full max-w-2xl mx-auto p-4  mb-10 md:mb-28 space-y-8">
      <Image
        src="/doodle/brainworm.webp"
        width={75}
        height={75}
        radius="full"
      />
      <h1 className="text-xl md:text-3xl text-center font-light  text-black tracking-looser">
        What can I help you Research?
      </h1>

      <div className="w-full">
        <PromptForm />
      </div>
    </div>
  );
}
