import { createFileRoute, useRouter } from "@tanstack/react-router";
import { useEffect } from "react";
import { Logo } from "../components/Icons";
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
    <div className="s h-screen w-full flex items-center justify-center bg-primary ">
      <div className="relative mb-8">
        <div
          className="absolute inset-0 rounded-full animate-spin border-1 border-solid border-secondary border-t-transparent"
          style={{
            width: "calc(100% + 15px)",
            height: "calc(100% + 15px)",
            left: "-10px",
            top: "-10px",
          }}
        ></div>

        <div className="relative rounded-full bg-primary  flex items-center justify-center">
          <Logo className="w-64 h-64" />
        </div>
      </div>
    </div>
  );
}
