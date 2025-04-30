import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { HeroUIProvider } from "@heroui/react";
import { GoogleOAuthProvider } from "@react-oauth/google";
import type { ReactNode } from "react";
import { config } from "../config";

const queryClient = new QueryClient();

export default function Providers({ children }: { children: ReactNode }) {
	return (
		<HeroUIProvider>
			<QueryClientProvider client={queryClient}>
				<GoogleOAuthProvider clientId={config.googleClientId}>
					{children}
				</GoogleOAuthProvider>
			</QueryClientProvider>
		</HeroUIProvider>
	);
}
