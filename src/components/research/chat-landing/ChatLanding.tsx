import PromptForm from "./prompt-form";
import { LogoIcon } from "../../Icons";

export default function ChatLanding() {
  return (
    <div className="flex flex-col items-center w-full max-w-2xl mx-auto p-4  mb-10 md:mb-28 space-y-10">
      <div className="flex flex-col justify-center items-center gap-5">
        <LogoIcon className="rounded-full w-16 h-16" />
        <h1 className=" text-xl md:text-3xl text-center font-light   text-black tracking-tight">
          What can I help you Research?
        </h1>
      </div>

      <div className="w-full">
        <PromptForm />
      </div>
    </div>
  );
}
