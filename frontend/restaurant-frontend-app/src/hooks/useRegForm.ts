import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { useAppDispatch } from "@/app/hooks";
import { register } from "@/app/thunks/userThunks";
import { useNavigate } from "react-router";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";

export const useRegForm = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [passwordTouched] = useState(false);

  const passwordSchema = z.string().superRefine((value, ctx) => {
    if (!/[A-Z]/.test(value)) {
      ctx.addIssue({
        code: "custom",
        message: "At least one uppercase letter required",
      });
    }
    if (!/[a-z]/.test(value)) {
      ctx.addIssue({
        code: "custom",
        message: "At least one lowercase letter required",
      });
    }
    if (!/[0-9]/.test(value)) {
      ctx.addIssue({
        code: "custom",
        message: "At least one number required",
      });
    }
    if (!/[!@#$%^&*()\-+_=[\]{};:'",<.>/?\\|]/.test(value)) {
      ctx.addIssue({
        code: "custom",
        message: "At least one character required",
      });
    }
    if (value.length < 8 || value.length > 16) {
      ctx.addIssue({
        code: "custom",
        message: "Password must be 8-16 characters long",
      });
    }
  });

  const formSchema = z
    .object({
      firstName: z
        .string()
        .min(1, { message: "Enter your first name" })
        .max(50, {
          message: "First name must be up to 50 characters",
        })
        .regex(/^[A-Za-z-']*$/, {
          message: "Only Latin letters, hyphens, and apostrophes are allowed",
        }),
      lastName: z
        .string()
        .min(1, { message: "Enter your last name" })
        .max(50, {
          message: "Last name must be up to 50 characters",
        })
        .regex(/^[A-Za-z-']*$/, {
          message: "Only Latin letters, hyphens, and apostrophes are allowed",
        }),
      email: z.string().email({
        message:
          "Invalid email address. Please ensure it follows the format: username@domain.com",
      }),
      password: passwordSchema,
      confirmPassword: z.string(),
    })
    .refine(
      (data) => {
        const passwordResult = passwordSchema.safeParse(data.password);
        if (!passwordResult.success) {
          return false;
        }
        return data.password === data.confirmPassword;
      },
      {
        message: "Confirm password must match new password",
        path: ["confirmPassword"],
      },
    );

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
    mode: "all",
    criteriaMode: "all",
  });

  const passwordWatch = form.watch("password");

  useEffect(() => {
    if (!passwordWatch && !passwordTouched) return;
    form.trigger("confirmPassword");
  }, [passwordWatch, form]);

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    try {
      const result = await dispatch(register(values)).unwrap();
      console.log("Registration successful:", result);
      navigate("/signin");
    } catch (error) {
      if (error instanceof Error) {
        console.error("Registration failed:", error);
        toast.error(error.message);
      }
    }
  };

  return { form, onSubmit, formSchema };
};
