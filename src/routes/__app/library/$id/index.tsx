import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/__app/library/$id/')({
  component: RouteComponent,
})

function RouteComponent() {
  return <div>Hello "/__app/library/$id/"!</div>
}
