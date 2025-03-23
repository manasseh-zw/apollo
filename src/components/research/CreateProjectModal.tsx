import {
  Modal,
  ModalContent,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
  useDisclosure,
  Checkbox,
  Input,
  Link,
  Textarea,
  Alert,
} from "@heroui/react";
import { useMutation } from "@tanstack/react-query";
import { Plus } from "lucide-react";
import { useState } from "react";
import { createNewProject } from "../../services/research.service";
import { ResearchProjectResponse } from "../../types/research";
import { ApiResponse } from "../../types/api";
import { useRouter } from "@tanstack/react-router";
import { useForm } from "@tanstack/react-form";

export default function CreateProjectModal() {
  const router = useRouter();
  const { isOpen, onOpen, onOpenChange } = useDisclosure();

  const [serverErrors, setServerErrors] = useState<string[]>([]);

  const createProjectMutation = useMutation({
    mutationFn: (data: { title: string; description: string }) =>
      createNewProject(data),
    onSuccess: (response: ApiResponse<ResearchProjectResponse>) => {
      if (response.success) {
        setServerErrors([]);
        if (response.data != null) {
          router.navigate({ to: `/research/${response.data.id}` });
        }
      } else if (response.errors) {
        setServerErrors(response.errors);
      }
    },
    onError: (error: any) => {
      if (error.message) {
        setServerErrors([error.message]);
      } else if (error.errors && Array.isArray(error.errors)) {
        setServerErrors(error.errors);
      } else {
        setServerErrors(["An unexpected error occurred. Please try again."]);
      }
    },
  });

  const form = useForm({
    defaultValues: {
      title: "",
      description: "",
    },
    onSubmit: async ({ value }) => {
      setServerErrors([]);
      createProjectMutation.mutate(value);
    },
  });

  return (
    <>
      <Button color="primary" onPress={onOpen} endContent={<Plus />}>
        New Project
      </Button>
      <Modal
        isOpen={isOpen}
        placement="top-center"
        onOpenChange={onOpenChange}
        backdrop="blur"
      >
        <ModalContent>
          <form
            onSubmit={(e) => {
              e.preventDefault();
              e.stopPropagation();
              form.handleSubmit();
            }}
          >
            <ModalHeader className="flex flex-col gap-1">
              New Project
            </ModalHeader>
            <ModalBody>
              {serverErrors.length > 0 && (
                <Alert
                  color="danger"
                  title="There were errors with your submission"
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
              <form.Field name="title">
                {(field) => (
                  <Input
                    min={3}
                    value={field.state.value}
                    isRequired
                    onChange={(e) => field.handleChange(e.target.value)}
                    isDisabled={createProjectMutation.isPending}
                    label="Title"
                    variant="bordered"
                  />
                )}
              </form.Field>
              <form.Field name="description">
                {(field) => (
                  <Textarea
                    min={3}
                    isRequired
                    value={field.state.value}
                    onChange={(e) => field.handleChange(e.target.value)}
                    isDisabled={createProjectMutation.isPending}
                    label="Description"
                    variant="bordered"
                  />
                )}
              </form.Field>
            </ModalBody>
            <ModalFooter>
              <Button
                type="submit"
                color="primary"
                disabled={createProjectMutation.isPending}
                isLoading={createProjectMutation.isPending}
              >
                Create
              </Button>
            </ModalFooter>
          </form>
        </ModalContent>
      </Modal>
    </>
  );
}
