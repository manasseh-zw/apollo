import { createFileRoute, Outlet } from "@tanstack/react-router";
import AppSidebar from "../components/layout/AppSidebar";

export const Route = createFileRoute("/__app")({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <main className="h-screen w-full overflow-hidden flex flex-row md:bg-primary">
      <AppSidebar />
      <div className="w-full overflow-y-auto rounded-2xl shadow-sm bg-content1 md:border-1 md:border-content3 m-1">
        <Outlet />
      </div>
    </main>
  );
}
