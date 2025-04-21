import ChatLandingPromptForm from "./ChatLandingPromptForm";
import { Logo } from "../../Icons";

export default function ChatLanding() {
  return (
    <div className="flex flex-col items-center w-full max-w-xl mx-auto p-4  mb-10 space-y-9">
      <div className="flex flex-col justify-center items-center gap-5">
        <Logo height={38} width={38} />
        <div className=" flex flex-col text-center text-black tracking-tight gap-1 ">
          <h1 className=" text-xl md:text-3xl">Hey! I'm Apollo</h1>
          <small className="font-light text-medium">
            Ask. Explore. Understand.
          </small>
        </div>
      </div>

      <div className="w-full">
        <ChatLandingPromptForm onStartConversation={function (id: string, query: string): void {
          throw new Error("Function not implemented.");
        } } />
      </div>
    </div>
  );
}
