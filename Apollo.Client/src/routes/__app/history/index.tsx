import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/__app/history/')({
  component: RouteComponent,
})

function RouteComponent() {
  return <div>Hello "/__app/history/"!</div>
}
