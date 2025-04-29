import { Outlet, createRootRoute } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools";
import { authActions, store } from "../lib/state/store";
import { getSignedInUser } from "../lib/services/auth.service";

export const Route = createRootRoute({
  component: RootComponent,
  loader: async () => {
    const { isLoading } = store.state.authState;

    if (isLoading) {
      try {
        const response = await getSignedInUser();
        
        if (response.success && response.data) {
          authActions.setUser(response.data);
        } else {
          authActions.clearUser();
        }
      } catch (error) {
        console.error("Not authenticated:", error);
        authActions.clearUser();
      } finally {
        authActions.setLoading(false);
      }
    }
    return null;
  },
});

function RootComponent() {
  return (
    <main className="font-rubik ">
      <Outlet />
      {!import.meta.env.PROD && (
        <TanStackRouterDevtools position="bottom-right" />
      )}
    </main>
  );
}
