import { TanStackRouterVite } from "@tanstack/router-plugin/vite";
import viteReact from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// https://vitejs.dev/config/
export default defineConfig({
	plugins: [TanStackRouterVite({ autoCodeSplitting: true }), viteReact()],
	test: {
		globals: true,
		environment: "jsdom",
	},
});
