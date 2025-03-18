import { Button } from "@/components/ui/";
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
  CustomLink,
} from "@/components/ui/";
import { ComponentProps } from "react";
import { cn } from "@/lib/utils";
import { PasswordField } from "../ui/PasswordField";
import { useLoginForm } from "@/hooks/useLoginForm";
import { Link } from "react-router";

export function LoginForm({ className, ...props }: ComponentProps<"form">) {
  const { form, onSubmit } = useLoginForm();

  return (
    <Form {...form}>
      <form
        className={cn(className, "flex flex-col")}
        onSubmit={form.handleSubmit(onSubmit)}
        aria-labelledby="login-form-title"
        {...props}
      >
        <Text variant="blockTitle" className="uppercase">
          Welcome back
        </Text>
        <Text variant="h2" tag="h1">
          Sign In to Your Account
        </Text>
        <FormFieldSet className="flex flex-col mt-[2rem] md:mt-[4rem]">
          <FormField
            control={form.control}
            name="email"
            render={({ field, formState }) => (
              <FormItem>
                <FormLabel>Email</FormLabel>
                <FormControl>
                  <Input
                    isInvalid={Boolean(formState.errors.email?.message)}
                    placeholder="Enter your Email"
                    {...field}
                  />
                </FormControl>
                {formState.errors.email?.message ? (
                  <FormMessage />
                ) : (
                  <FormDescription>e.g. username@domain.com</FormDescription>
                )}
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="password"
            render={({ field, fieldState, formState }) => {
              return (
                <FormItem className="mt-[1.5rem]">
                  <FormLabel>Password</FormLabel>
                  <FormControl>
                    <PasswordField
                      isInvalid={Boolean(
                        Object.keys(fieldState.error ?? {}).length,
                      )}
                      placeholder="Enter your Password"
                      {...field}
                    />
                  </FormControl>
                  {formState.errors.password?.message ? (
                    <FormMessage />
                  ) : (
                    <FormDescription></FormDescription>
                  )}
                </FormItem>
              );
            }}
          />
        </FormFieldSet>
        <Button type="submit" className="mt-[3rem] md:mt-[3rem]">
          Sign In
        </Button>
        <Text className="mt-[16px]" variant="caption">
          Don’t have an account?{" "}
          <CustomLink asChild>
            <Link to="/signup">Create an Account</Link>
          </CustomLink>
        </Text>
      </form>
    </Form>
  );
}
