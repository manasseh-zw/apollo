import React from "react";
import { Slider, Button } from "@heroui/react";
import { ChevronDown, Type } from "lucide-react";

interface FontSizeControllerProps {
  fontSize: number;
  onFontSizeChange: (size: number) => void;
}

export default function FontSizeController({
  fontSize,
  onFontSizeChange,
}: FontSizeControllerProps) {
  const [isExpanded, setIsExpanded] = React.useState(false);

  const handleFontSizeChange = (value: number | number[]) => {
    const newSize = Array.isArray(value) ? value[0] : value;
    onFontSizeChange(newSize);
  };

  return (
    <div className="absolute bottom-4 left-0 right-0 flex justify-center z-50 px-4">
      <div
        className={`transition-all duration-300 ${isExpanded ? "w-full max-w-md" : "w-auto"}`}
      >
        <div className="bg-primary rounded-full border border-zinc-800">
          {isExpanded ? (
            <div className="flex items-center gap-4 px-5 py-3">
              <Button
                isIconOnly
                size="sm"
                radius="full"
                onPress={() => setIsExpanded(false)}
                className="text-zinc-400 hover:text-white bg-zinc-900"
              >
                <ChevronDown width={18} />
              </Button>

              <span className="text-sm font-medium text-zinc-300">
                Font Size
              </span>

              <div className="w-full max-w-[180px]">
                <Slider
                  size="sm"
                  color="primary"
                  value={fontSize}
                  minValue={12}
                  maxValue={24}
                  step={1}
                  onChange={handleFontSizeChange}
                  aria-label="Font size"
                  className="z-0"
                />
              </div>

              <span className="text-sm font-medium min-w-[40px] text-right text-zinc-300">
                {fontSize}px
              </span>
            </div>
          ) : (
            <Button
              size="sm"
              variant="flat"
              radius="full"
              startContent={<Type width={16} />}
              endContent={<span className="ml-1">{fontSize}px</span>}
              onPress={() => setIsExpanded(true)}
              className="px-4 py-2 bg-primary text-white hover:bg-zinc-900"
            >
              Font Size
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
