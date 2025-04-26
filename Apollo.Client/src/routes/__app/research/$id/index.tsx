import { createFileRoute } from "@tanstack/react-router";
import AgentChat from "../../../../components/research/AgentChat";
import ResearchFeed from "../../../../components/research/feed/ResearchFeed";
import ResearchReport from "../../../../components/research/report/ResearchReport";
import { useState } from "react";

export const Route = createFileRoute("/__app/research/$id/")({
  component: RouteComponent,
});

const mockResearchData = {
  status: "completed",
  report: {
    title: "Quantum Computing: Future Implications",
    content: `
# Executive Summary

Quantum computing represents a paradigm shift in computational capabilities, leveraging quantum mechanical phenomena to perform operations on data. Unlike classical computers that use bits (0s and 1s), quantum computers use quantum bits or qubits that can exist in multiple states simultaneously through superposition.

This research report examines the current state of quantum computing technology, its potential applications across various industries, and the implications for future technological development. Our analysis indicates that while quantum computing is still in its early stages, significant advancements have been made in recent years that suggest a transformative impact on fields ranging from cryptography to drug discovery.

## Key Findings

The quantum computing market is projected to grow from $866 million in 2023 to approximately $4.375 billion by 2028, representing a compound annual growth rate (CAGR) of 38.3%. This growth is driven by increasing investments from both private and public sectors, as well as the expanding range of potential applications.

Major technology companies including IBM, Google, Microsoft, and Amazon have established significant quantum computing initiatives. IBM's 433-qubit Osprey processor, announced in 2022, represents one of the most advanced quantum systems currently available, while Google's 53-qubit Sycamore processor demonstrated quantum supremacy in 2019 by performing a calculation that would be practically impossible for classical supercomputers.

## Potential Applications

Quantum computing shows particular promise in several key areas:

* **Cryptography:** Quantum computers could potentially break many of the encryption algorithms currently used to secure internet communications, necessitating the development of quantum-resistant cryptographic methods.
* **Drug Discovery:** Quantum computers could dramatically accelerate the process of discovering new drugs by simulating molecular interactions with unprecedented accuracy.
* **Optimization Problems:** Complex optimization challenges in logistics, finance, and energy distribution could be solved more efficiently using quantum algorithms.
* **Material Science:** Quantum computers could help design new materials with specific properties by accurately modeling atomic interactions.
    `,
  },
};

function RouteComponent() {
  const [researchData] = useState(mockResearchData);

  if (researchData.status === "completed") {
    return <ResearchReport />;
  }

  return (
    <main className="w-full h-full grid grid-cols-12 gap-4 bg-white p-1">
      <div className="h-full col-span-8 border-r border-gray-200">
        <ResearchFeed />
      </div>
      <div className="h-full col-span-4 flex flex-col overflow-hidden py-2">
        <AgentChat />
      </div>
    </main>
  );
}
