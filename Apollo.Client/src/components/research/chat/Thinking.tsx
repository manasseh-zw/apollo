export default function Thinking() {
  return (
    <div className="flex flex-row gap-1">
      <div className="w-2 h-2 rounded-full bg-secondary animate-bounce [animation-delay:-.3s]"></div>
      <div className="w-2 h-2 rounded-full bg-primary-300 animate-bounce [animation-delay:-.4s]"></div>
      <div className="w-2 h-2 rounded-full bg-secondary animate-bounce [animation-delay:-.5s]"></div>
    </div>
  );
}
