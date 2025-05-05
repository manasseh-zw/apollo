import { Button, Link } from "@heroui/react";
import { ArrowUpRight } from "lucide-react";

export default function Banner() {
  return (
    <div className="flex w-full items-center justify-center gap-x-3 border-b-1 border-divider bg-primary px-6 py-1.5 sm:px-3.5">
      <p className="text-small text-primary-foreground text-center">
        <Link className="text-inherit" href="#">
          Try it now for absolutely nothing! Ask, Understand, Explore.&nbsp;
        </Link>
      </p>
      <Button
        as={Link}
        className="group relative h-7 overflow-hidden bg-primary-foreground text-small font-medium text-primary"
        color="default"
        endContent={
          <ArrowUpRight
            className="flex-none outline-none transition-transform group-data-[hover=true]:translate-x-0.5 [&>path]:stroke-[2]"
            width={14}
          />
        }
        href="/auth/sign-up"
        radius="full"
      >
        Signup
      </Button>
    </div>
  );
}
