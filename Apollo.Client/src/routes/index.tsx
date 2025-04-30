import { createFileRoute, useRouter } from "@tanstack/react-router";
import { useEffect } from "react";
import { LogoLight } from "../components/Icons";
import { store } from "../lib/state/store";

export const Route = createFileRoute("/")({
  component: HomeComponent,
});

function HomeComponent() {
  const router = useRouter();
  const { isAuthenticated, isLoading } = store.state.authState;

  useEffect(() => {
    if (!isLoading) {
      router.navigate({
        to: isAuthenticated ? "/research" : "/auth/sign-in",
      });
    }
  }, [isLoading, isAuthenticated]);

  return (
    <div className="s h-screen w-full flex items-center justify-center bg-black ">
      <div className="relative mb-8">
        <div className="relative rounded-full   flex items-center justify-center">
          <LogoLight className="w-20 h-20" />
        </div>
      </div>
    </div>
  );
}
