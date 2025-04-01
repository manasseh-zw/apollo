import React from "react";
import { useControlledState } from "@react-stately/utils";
import { m, LazyMotion, domAnimation } from "framer-motion";
import { cn } from "@heroui/react";

export type VerticalStepperProps = {
  className?: string;
  description?: React.ReactNode;
  title?: React.ReactNode;
};

export interface VerticalStepsProps
  extends React.HTMLAttributes<HTMLButtonElement> {
  steps?: VerticalStepperProps[];
  color?:
    | "default"
    | "primary"
    | "secondary"
    | "success"
    | "warning"
    | "danger";
  currentStep?: number;
  defaultStep?: number;
  hideProgressBars?: boolean;
  className?: string;
  stepClassName?: string;
  onStepChange?: (stepIndex: number) => void;
}

function CheckIcon(props: React.ComponentProps<"svg">) {
  return (
    <svg
      {...props}
      fill="none"
      stroke="currentColor"
      strokeWidth={2}
      viewBox="0 0 24 24"
    >
      <m.path
        animate={{ pathLength: 1 }}
        d="M5 13l4 4L19 7"
        initial={{ pathLength: 0 }}
        strokeLinecap="round"
        strokeLinejoin="round"
        transition={{
          delay: 0.2,
          type: "tween",
          ease: "easeOut",
          duration: 0.3,
        }}
      />
    </svg>
  );
}

const VerticalStepper = React.forwardRef<HTMLButtonElement, VerticalStepsProps>(
  (
    {
      color = "primary",
      steps = [],
      defaultStep = 0,
      onStepChange,
      currentStep: currentStepProp,
      hideProgressBars = false,
      stepClassName,
      className,
      ...props
    },
    ref
  ) => {
    const [currentStep, setCurrentStep] = useControlledState(
      currentStepProp,
      defaultStep,
      onStepChange
    );

    const colors = React.useMemo(() => {
      const baseColors = {
        default: "bg-default text-default-foreground",
        primary: "bg-primary-400 text-primary-foreground",
        secondary: "bg-secondary text-secondary-foreground",
        success: "bg-success text-success-foreground",
        warning: "bg-warning text-warning-foreground",
        danger: "bg-danger text-danger-foreground",
      };

      const borderColors = {
        default: "border-default",
        primary: "border-primary",
        secondary: "border-secondary",
        success: "border-success",
        warning: "border-warning",
        danger: "border-danger",
      };

      return {
        active: borderColors[color],
        complete: baseColors[color],
        inactive: "border-default-400 text-default-500",
        progressBar: {
          active: borderColors[color],
          inactive: "bg-default-300",
        },
      };
    }, [color]);

    return (
      <nav aria-label="Progress" className="max-w-fit">
        <ol className={cn("flex flex-col gap-y-3", className)}>
          {steps?.map((step, stepIdx) => {
            const status =
              currentStep === stepIdx
                ? "active"
                : currentStep < stepIdx
                  ? "inactive"
                  : "complete";

            return (
              <li key={stepIdx} className="relative">
                <div className="flex w-full max-w-full items-center">
                  <button
                    ref={ref}
                    aria-current={status === "active" ? "step" : undefined}
                    className={cn(
                      "group flex w-full cursor-pointer items-center justify-center gap-4 rounded-large px-3 py-2.5",
                      stepClassName
                    )}
                    onClick={() => setCurrentStep(stepIdx)}
                    {...props}
                  >
                    <div className="flex h-full items-center">
                      <LazyMotion features={domAnimation}>
                        <div className="relative">
                          <m.div
                            animate={status}
                            className={cn(
                              "relative flex h-[28px] w-[28px] items-center justify-center rounded-full border-2",
                              {
                                [colors.complete]: status === "complete",
                                [colors.active]: status === "active",
                                [colors.inactive]: status === "inactive",
                                "shadow-sm": status === "complete",
                              }
                            )}
                            initial={false}
                            transition={{ duration: 0.25 }}
                          >
                            <div className="flex items-center justify-center">
                              {status === "complete" ? (
                                <CheckIcon className="h-5 w-5" />
                              ) : (
                                <span className="text-sm">{stepIdx + 1}</span>
                              )}
                            </div>
                          </m.div>
                        </div>
                      </LazyMotion>
                    </div>
                    <div className="flex-1 text-left">
                      <div>
                        <div
                          className={cn(
                            "text-medium font-medium transition-[color,opacity] duration-300 group-active:opacity-70",
                            {
                              "text-default-500": status === "inactive",
                              "text-default-foreground": status !== "inactive",
                            }
                          )}
                        >
                          {step.title}
                        </div>
                        <div
                          className={cn(
                            "text-tiny lg:text-small transition-[color,opacity] duration-300 group-active:opacity-70",
                            {
                              "text-default-400": status === "inactive",
                              "text-default-500": status !== "inactive",
                            }
                          )}
                        >
                          {step.description}
                        </div>
                      </div>
                    </div>
                  </button>
                </div>
                {stepIdx < steps.length - 1 && !hideProgressBars && (
                  <div
                    aria-hidden="true"
                    className="pointer-events-none absolute left-[.60rem] top-[calc(64px_*_var(--idx)_+_1)] flex h-1/2 -translate-y-1/3 items-center px-4"
                    style={
                      {
                        "--idx": stepIdx,
                      } as React.CSSProperties
                    }
                  >
                    <div
                      className={cn(
                        "relative h-full w-0.5 transition-colors duration-300",
                        colors.progressBar.inactive,
                        "after:absolute after:block after:h-0 after:w-full after:transition-[height] after:duration-300 after:content-['']",
                        colors.progressBar.active,
                        {
                          "after:h-full": stepIdx < currentStep,
                        }
                      )}
                    />
                  </div>
                )}
              </li>
            );
          })}
        </ol>
      </nav>
    );
  }
);

export default VerticalStepper;
