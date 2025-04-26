import { useState } from "react";
import {
  Button,
  ButtonGroup,
  Dropdown,
  DropdownTrigger,
  DropdownMenu,
  DropdownItem,
} from "@heroui/react";
import { ChevronDownIcon, Download } from "lucide-react";
import { MSWord, MSPowerPoint, PDFIcon } from "../../Icons";
import FontSizeController from "./FontSizeController";

export default function ResearchReader() {
  const [fontSize, setFontSize] = useState(16);
  const [selectedExportOption, setSelectedExportOption] = useState(
    new Set(["pdf"])
  );

  const exportOptions = {
    pdf: { text: "Export as PDF", icon: PDFIcon },
    word: { text: "Export as Word", icon: MSWord },
    powerpoint: { text: "Export as PowerPoint", icon: MSPowerPoint },
  };

  // Get the currently selected export option
  const selectedOption = Array.from(
    selectedExportOption
  )[0] as keyof typeof exportOptions;

  return (
    <div className="flex flex-col h-full bg-white">
      {/* Top Header */}
      <header className="pt-4">
        <div className="max-w-3xl mx-auto flex justify-end">
          <ButtonGroup variant="flat">
            <Button className="bg-primary text-primary-foreground">
              {exportOptions[selectedOption]?.text || "Export"}
            </Button>
            <Dropdown placement="bottom-end">
              <DropdownTrigger>
                <Button
                  isIconOnly
                  className="bg-primary text-primary-foreground hover:bg-primary/95"
                >
                  <ChevronDownIcon />
                </Button>
              </DropdownTrigger>
              <DropdownMenu
                disallowEmptySelection
                aria-label="Export options"
                selectedKeys={selectedExportOption}
                color="primary"
                selectionMode="single"
                //@ts-ignore
                onSelectionChange={setSelectedExportOption}
              >
                <DropdownItem
                  key="word"
                  startContent={<MSWord className="h-5 w-5" />}
                >
                  Export as Word
                </DropdownItem>
                <DropdownItem
                  key="powerpoint"
                  startContent={<MSPowerPoint className="h-5 w-5" />}
                >
                  Export as PowerPoint
                </DropdownItem>
                <DropdownItem
                  key="pdf"
                  startContent={<PDFIcon className="h-5 w-5" />}
                >
                  Export as PDF
                </DropdownItem>
              </DropdownMenu>
            </Dropdown>
          </ButtonGroup>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 overflow-auto p-8 md:px-16 lg:px-24 pb-24 font-geist">
        <div
          className="max-w-3xl mx-auto prose prose-slate"
          style={{ fontSize: `${fontSize}px` }}
        >
          <h2>Executive Summary</h2>
          <p>
            Quantum computing represents a paradigm shift in computational
            capabilities, leveraging quantum mechanical phenomena to perform
            operations on data. Unlike classical computers that use bits (0s and
            1s), quantum computers use quantum bits or qubits that can exist in
            multiple states simultaneously through superposition.
          </p>

          <p>
            This research report examines the current state of quantum computing
            technology, its potential applications across various industries,
            and the implications for future technological development. Our
            analysis indicates that while quantum computing is still in its
            early stages, significant advancements have been made in recent
            years that suggest a transformative impact on fields ranging from
            cryptography to drug discovery.
          </p>

          <h2>Key Findings</h2>
          <p>
            The quantum computing market is projected to grow from $866 million
            in 2023 to approximately $4.375 billion by 2028, representing a
            compound annual growth rate (CAGR) of 38.3%. This growth is driven
            by increasing investments from both private and public sectors, as
            well as the expanding range of potential applications.
          </p>

          <p>
            Major technology companies including IBM, Google, Microsoft, and
            Amazon have established significant quantum computing initiatives.
            IBM's 433-qubit Osprey processor, announced in 2022, represents one
            of the most advanced quantum systems currently available, while
            Google's 53-qubit Sycamore processor demonstrated quantum supremacy
            in 2019 by performing a calculation that would be practically
            impossible for classical supercomputers.
          </p>

          <h2>Potential Applications</h2>
          <p>
            Quantum computing shows particular promise in several key areas:
          </p>

          <ul>
            <li>
              <strong>Cryptography:</strong> Quantum computers could potentially
              break many of the encryption algorithms currently used to secure
              internet communications, necessitating the development of
              quantum-resistant cryptographic methods.
            </li>
            <li>
              <strong>Drug Discovery:</strong> Quantum computing could
              dramatically accelerate the process of discovering new drugs by
              simulating molecular interactions with unprecedented accuracy.
            </li>
            <li>
              <strong>Optimization Problems:</strong> Complex optimization
              challenges in logistics, finance, and energy distribution could be
              solved more efficiently using quantum algorithms.
            </li>
            <li>
              <strong>Material Science:</strong> Quantum computers could help
              design new materials with specific properties by accurately
              modeling atomic interactions.
            </li>
          </ul>
        </div>
      </main>

      {/* Font Size Controller */}
      <div className="relative">
        <FontSizeController
          fontSize={fontSize}
          onFontSizeChange={setFontSize}
        />
      </div>
    </div>
  );
}
