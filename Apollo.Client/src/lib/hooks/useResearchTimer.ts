import { useState, useEffect } from "react";

function formatDuration(seconds: number): string {
  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const remainingSeconds = seconds % 60;

  const parts = [];
  if (hours > 0) parts.push(`${hours}h`);
  if (minutes > 0) parts.push(`${minutes}m`);
  parts.push(`${remainingSeconds}s`);

  return parts.join(" ");
}

export function useResearchTimer(startTime: string) {
  const [duration, setDuration] = useState("0s");

  useEffect(() => {
    const startTimestamp = new Date(startTime).getTime();

    const updateDuration = () => {
      const now = new Date().getTime();
      const diffSeconds = Math.floor((now - startTimestamp) / 1000);
      setDuration(formatDuration(diffSeconds));
    };

    // Update immediately
    updateDuration();

    // Then update every second
    const interval = setInterval(updateDuration, 1000);

    return () => clearInterval(interval);
  }, [startTime]);

  return duration;
}
