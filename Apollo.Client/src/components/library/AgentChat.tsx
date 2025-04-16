import { Avatar } from "@heroui/react";

type Agent = {
  name: string;
  role: string;
  avatar: string;
};

type Message = {
  id: number;
  text: string;
  agent: Agent;
  timestamp: string;
};

const agents = {
  apollo: {
    name: "Apollo",
    role: "Research Orchestrator",
    avatar: "/agents/agent1.jpg",
  },
  hermes: {
    name: "Hermes",
    role: "Query Explorer",
    avatar: "/agents/agent2.jpg",
  },
  athena: {
    name: "Athena",
    role: "Web Navigator",
    avatar: "/agents/agent3.jpg",
  },
  theia: {
    name: "Theia",
    role: "Truth Seeker",
    avatar: "/agents/agent4.jpg",
  },
  chronos: {
    name: "Chronos",
    role: "Knowledge Curator",
    avatar: "/agents/agent5.jpg",
  },
};

// Sample messages for demonstration
const messages: Message[] = [
  {
    id: 1,
    agent: agents.apollo,
    text: "I've received a fascinating query about quantum computing's impact on modern cryptography. Hermes, could you help us explore the most relevant search paths?",
    timestamp: "12:01",
  },
  {
    id: 2,
    agent: agents.hermes,
    text: "On it, Apollo! I'm weaving through the digital pathways... 🔍 Found some promising trails: recent breakthroughs in quantum algorithms, post-quantum encryption standards, and real-world implementation challenges. Where should we focus first?",
    timestamp: "12:02",
  },
  {
    id: 3,
    agent: agents.apollo,
    text: "Let's prioritize the recent breakthroughs. Athena, would you navigate the academic realm for us? Focus on peer-reviewed papers from the last two years.",
    timestamp: "12:02",
  },
  {
    id: 4,
    agent: agents.athena,
    text: "Already soaring through IEEE and arXiv repositories 🦉 I've discovered several groundbreaking papers on Shor's algorithm optimizations. Theia, you might want to examine these findings closely.",
    timestamp: "12:03",
  },
  {
    id: 5,
    agent: agents.theia,
    text: "Thank you, Athena! ✨ These papers are fascinating. I'm seeing a consistent pattern in the research - quantum computers with just 4000 qubits could potentially break current RSA encryption. Chronos, this seems significant for our timeline analysis.",
    timestamp: "12:04",
  },
  {
    id: 6,
    agent: agents.chronos,
    text: "Indeed, Theia! This aligns with the temporal patterns I'm observing. The pace of quantum computing development has accelerated significantly since 2022. Apollo, shall I prepare a chronological evolution of these breakthroughs?",
    timestamp: "12:05",
  },
  {
    id: 7,
    agent: agents.apollo,
    text: "Perfect timing, everyone! 🌟 Let's synthesize these findings. I'm seeing a clear narrative forming about the urgency of quantum-resistant cryptography. Hermes, could you explore some practical implementation cases next?",
    timestamp: "12:06",
  },
];

export default function AgentChat() {
  return (
    <div className="flex flex-col h-full bg-content1 rounded-large overflow-hidden">
      <div className="p-3  border-content4 flex items-center gap-3">
        <div className="flex -space-x-2">
          {Object.values(agents).map((agent) => (
            <Avatar
              key={agent.name}
              src={agent.avatar}
              className="w-6 h-6 ring-2 ring-content1"
            />
          ))}
        </div>
        <h2 className="text-small font-medium">Agent Collaboration</h2>
      </div>
      <div className="flex-1 p-3 space-y-4 overflow-auto">
        {messages.map((message) => (
          <div
            key={message.id}
            className="flex items-start gap-2 group animate-in fade-in slide-in-from-bottom-2 duration-300"
          >
            <Avatar
              src={message.agent.avatar}
              className="w-7 h-7 transition-transform group-hover:scale-105"
            />
            <div className="flex-1 min-w-0">
              <div className="flex items-baseline gap-2 flex-wrap">
                <span className="font-medium text-small truncate">
                  {message.agent.name}
                </span>
                <span className="text-tiny text-default-500 truncate">
                  {message.agent.role}
                </span>
                <span className="text-tiny text-default-400 ml-auto flex-shrink-0">
                  {message.timestamp}
                </span>
              </div>
              <p className="mt-1 text-small text-foreground-600 leading-relaxed break-words">
                {message.text}
              </p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
