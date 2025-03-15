import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { Button } from "@/components/ui/Button";
import {
  Form,
  FormControl,
  FormDescription,
  FormFieldSet,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
  Input,
  Text,
} from "@/components/ui/";
import { ComponentProps } from "react";
import { cn } from "@/lib/utils";

const formSchema = z.object({
  email: z
    .string()
    .min(1, { message: "This field has to be filled." })
    .email("This is not a valid email.")
    .refine(async (e) => {
      return await checkIfEmailIsValid(e);
    }, "This email is not in our database"),
  password: z.string(),
});

export default function LoginForm({
  className,
  ...props
}: ComponentProps<"form">) {
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  function onSubmit(values: z.infer<typeof formSchema>) {
    console.log(values);
  }

  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className={cn(className)}
        aria-labelledby="login-form-title"
      >
        <div className="max-w-[496px]">
          <Text variant="blockTitle">Welcome back</Text>
          <Text variant="h2" as="h1" id="login-form-title">
            Sign In to Your Account
          </Text>
          <FormFieldSet className="flex flex-col gap-y-[24px] mt-[64px]">
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input placeholder="Enter your Email" {...field} />
                  </FormControl>
                  <FormDescription className="text-neutral-400">
                    e.g. username@domain.com
                  </FormDescription>
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="password"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Password</FormLabel>
                  <FormControl>
                    <Input
                      type="password"
                      placeholder="Enter your Password"
                      {...field}
                    />
                  </FormControl>
                </FormItem>
              )}
            />
          </FormFieldSet>
          <Button type="submit" className="mt-[64px]">
            Sign In
          </Button>
          <Text className="mt-[16px]" variant="caption">
            Don’t have an account? Create an Account
          </Text>
        </div>
      </form>
    </Form>
  );
}
