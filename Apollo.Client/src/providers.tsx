import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { HeroUIProvider, ToastProvider } from "@heroui/react";
import { GoogleOAuthProvider } from "@react-oauth/google";
import type { ReactNode } from "react";
import { config } from "../config";
import { ResearchProvider } from "./contexts/ResearchContext";

const queryClient = new QueryClient();

export default function Providers({ children }: { children: ReactNode }) {
  return (
    <HeroUIProvider>
      <QueryClientProvider client={queryClient}>
        <GoogleOAuthProvider clientId={config.googleClientId}>
          <ResearchProvider>
            <ToastProvider />
            {children}
          </ResearchProvider>
        </GoogleOAuthProvider>
      </QueryClientProvider>
    </HeroUIProvider>
  );
}
