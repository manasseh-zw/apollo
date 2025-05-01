import { Alert, Input, Button, Divider } from "@heroui/react";
import { useGoogleLogin } from "@react-oauth/google";
import { useForm } from "@tanstack/react-form";
import { createFileRoute, useRouter, Link } from "@tanstack/react-router";
import { EyeClosed, Eye, Mail } from "lucide-react";
import { useState } from "react";
import { m, LazyMotion, domAnimation, AnimatePresence } from "framer-motion";
import { Google, Logo } from "../../components/Icons";
import { emailSignup, googleSignup } from "../../lib/services/auth.service";
import { authActions } from "../../lib/state/store";
import type { User } from "../../lib/types/user";
import type { ApiResponse } from "../../lib/utils/api";
import { publicOnlyLoader } from "../../lib/utils/loaders";
import { useMutation } from "@tanstack/react-query";

export const Route = createFileRoute("/auth/sign-up")({
  component: SignUp,
  loader: publicOnlyLoader,
});

function SignUp() {
  const router = useRouter();
  const [serverErrors, setServerErrors] = useState<string[]>([]);
  const [isVisible, setIsVisible] = useState(false);
  const [isFormVisible, setIsFormVisible] = useState(false);

  const variants = {
    visible: { opacity: 1, y: 0 },
    hidden: { opacity: 0, y: 10 },
  };

  const emailSignUpMutation = useMutation({
    mutationFn: (data: { email: string; password: string; username: string }) =>
      emailSignup(data),
    onSuccess: (response: ApiResponse<User>) => {
      if (response.success && response.data) {
        authActions.setUser(response.data);
        router.navigate({ to: "/research", replace: true });
      } else if (response.errors) {
        setServerErrors(response.errors);
      }
    },
    onError: (error: Error | { errors?: string[] }) => {
      if ("errors" in error && Array.isArray(error.errors)) {
        setServerErrors(error.errors);
      } else {
        //@ts-ignore
        setServerErrors([error.message || "An unexpected error occurred"]);
      }
    },
  });

  const googleMutation = useMutation({
    mutationFn: (accessToken: string) => googleSignup(accessToken),
    onSuccess: (response: ApiResponse<User>) => {
      if (response.success && response.data) {
        authActions.setUser(response.data);
        router.navigate({ to: "/research", replace: true });
      } else if (response.errors) {
        setServerErrors(response.errors);
      }
    },
    onError: (error: Error) => {
      setServerErrors([error.message || "Google sign-up failed"]);
    },
  });

  const handleGoogleSignIn = useGoogleLogin({
    onSuccess: (tokenResponse) => {
      googleMutation.mutate(tokenResponse.access_token);
    },
    onError: () => {
      setServerErrors(["Google authentication failed"]);
    },
    flow: "implicit",
  });

  const handleGuestSignIn = () => {
    router.navigate({ to: "/research", replace: true });
  };

  const form = useForm({
    defaultValues: {
      username: "",
      email: "",
      password: "",
    },
    onSubmit: async ({ value }) => {
      setServerErrors([]);
      emailSignUpMutation.mutate(value);
    },
  });

  const orDivider = (
    <div className="flex items-center gap-2">
      <Divider className="flex-1 " />
      <p className="shrink-0 text-tiny text-default-500">OR</p>
      <Divider className="flex-1 " />
    </div>
  );

  return (
    <div className="flex h-screen w-full items-center justify-center bg-content1">
      <div className="flex w-full max-w-xs flex-col gap-4 rounded-large mb-5 px-4 md:px-2">
        <div className="flex flex-col items-center pb-3 gap-3">
          <Logo width={40} height={40} />
          <p className="text-small text-secondary-700">
            Create an account to get started
          </p>
        </div>

        {serverErrors.length > 0 && (
          <Alert
            color="danger"
            title="Sign-up failed"
            description={
              <ul className="list-disc ml-5 mt-1 text-xs">
                {serverErrors.map((error, index) => (
                  <li key={index}>{error}</li>
                ))}
              </ul>
            }
            variant="bordered"
          />
        )}

        <LazyMotion features={domAnimation}>
          <AnimatePresence mode="wait">
            {isFormVisible ? (
              <m.form
                animate="visible"
                exit="hidden"
                initial="hidden"
                variants={variants}
                className="flex flex-col gap-4"
                onSubmit={(e) => {
                  e.preventDefault();
                  e.stopPropagation();
                  form.handleSubmit();
                }}
              >
                <form.Field name="username">
                  {(field) => (
                    <Input
                      min={3}
                      isRequired
                      label="Username"
                      type="text"
                      size="sm"
                      variant="flat"
                      value={field.state.value}
                      onChange={(e) => field.handleChange(e.target.value)}
                      isDisabled={emailSignUpMutation.isPending}
                    />
                  )}
                </form.Field>

                <form.Field name="email">
                  {(field) => (
                    <Input
                      isRequired
                      label="Email Address"
                      type="email"
                      variant="flat"
                      size="sm"
                      value={field.state.value}
                      onChange={(e) => field.handleChange(e.target.value)}
                      isDisabled={emailSignUpMutation.isPending}
                    />
                  )}
                </form.Field>

                <form.Field name="password">
                  {(field) => (
                    <Input
                      minLength={8}
                      isRequired
                      endContent={
                        <button
                          type="button"
                          onClick={() => setIsVisible(!isVisible)}
                          disabled={emailSignUpMutation.isPending}
                        >
                          {isVisible ? <EyeClosed /> : <Eye />}
                        </button>
                      }
                      label="Password"
                      type={isVisible ? "text" : "password"}
                      variant="flat"
                      size="sm"
                      value={field.state.value}
                      onChange={(e) => field.handleChange(e.target.value)}
                      isDisabled={emailSignUpMutation.isPending}
                    />
                  )}
                </form.Field>

                <Button
                  className="w-full"
                  color="primary"
                  type="submit"
                  isDisabled={emailSignUpMutation.isPending}
                  isLoading={emailSignUpMutation.isPending}
                >
                  Sign Up
                </Button>

                {orDivider}

                <Button
                  variant="bordered"
                  onPress={() => setIsFormVisible(false)}
                >
                  Other Sign Up options
                </Button>
              </m.form>
            ) : (
              <m.div
                animate="visible"
                exit="hidden"
                initial="hidden"
                variants={variants}
                className="flex flex-col gap-4"
              >
                <Button
                  className="w-full"
                  color="primary"
                  startContent={<Mail size={18} />}
                  onPress={() => setIsFormVisible(true)}
                >
                  Continue with Email
                </Button>
                <Button
                  className="w-full bg-primary-900/90 text-white"
                  variant="flat"
                  startContent={<Google width={18} height={18} />}
                  onPress={() => handleGoogleSignIn()}
                  isDisabled={googleMutation.isPending}
                  isLoading={googleMutation.isPending}
                >
                  Continue with Google
                </Button>
                {/* 
								<Button
									className="w-full text-primary"
									variant="flat"
									color="secondary"
									onPress={handleGuestSignIn}
								>
									Continue as Guest
								</Button> */}

                <p className="text-center text-small mt-3">
                  Already have an account?{" "}
                  <Link
                    to="/auth/sign-in"
                    className="underline text-primary-500"
                  >
                    Sign In
                  </Link>
                </p>
              </m.div>
            )}
          </AnimatePresence>
        </LazyMotion>
      </div>
    </div>
  );
}
