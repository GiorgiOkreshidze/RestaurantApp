import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

export const useLoginForm = () => {
  const formSchema = z.object({
    email: z
      .string()
      .nonempty({
        message:
          "Email address is required. Please enter your email to continue",
      })
      .email({
        message:
          "Invalid email address. Please ensure it follows the format: username@domain.com",
      }),
    password: z.string().nonempty({
      message: "Password is required. Please enter your password to continue.",
    }),
  });

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      password: "",
    },
    mode: "all",
    criteriaMode: "all",
  });

  function onSubmit(values: z.infer<typeof formSchema>) {
    console.log(values);
  }

  return { form, onSubmit, formSchema };
};
