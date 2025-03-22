import { Button } from "@heroui/react";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
	component: App,
});

function App() {
	return (
		<div>
			<p className="font-rubik">hello world</p>
			<Button color="primary">
				hello
			</Button>
		</div>
	);
}
