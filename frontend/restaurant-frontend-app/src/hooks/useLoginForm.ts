import { useAppDispatch } from "@/app/hooks";
import { getUserData, login } from "@/app/thunks/userThunks";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router";
import { z } from "zod";

export const useLoginForm = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

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

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    try {
      const result = await dispatch(login(values)).unwrap();
      const userData = await dispatch(getUserData()).unwrap();
      console.log("Login successful:", result);
      console.log("Userdata successful:", userData);
      navigate("/");
    } catch (error) {
      console.error("Login failed:", error);
    }
  };

  return { form, onSubmit, formSchema };
};
