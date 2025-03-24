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
  FormDescriptionCircled,
  Button,
  CustomLink,
} from "@/components/ui/";
import { ComponentProps, useState } from "react";
import { cn } from "@/lib/utils";
import { useRegForm } from "@/hooks/useRegForm";
import { PasswordField } from "@/components/ui/PasswordField";
import { Link } from "react-router";
import { useSelector } from "react-redux";
import { selectRegisterLoading } from "@/app/slices/userSlice";

export function RegForm({ className, ...props }: ComponentProps<"form">) {
  const { form, onSubmit } = useRegForm();
  const isLoading = useSelector(selectRegisterLoading);
  const [showPassword, setShowPassword] = useState(false);

  return (
    <Form {...form}>
      <form
        className={cn(className, "flex flex-col")}
        onSubmit={form.handleSubmit(onSubmit)}
        aria-labelledby="login-form-title"
        {...props}
      >
        <Text variant="blockTitle" className="uppercase">
          Let's get you started
        </Text>
        <Text variant="h2" tag="h1">
          Create an Account
        </Text>
        <FormFieldSet className="flex flex-col mt-[2rem] md:mt-[4rem]">
          <div className="flex  items-center gap-4">
            <FormField
              control={form.control}
              name="firstName"
              render={({ field, formState }) => (
                <FormItem className="">
                  <FormLabel>First Name</FormLabel>
                  <FormControl>
                    <Input
                      isInvalid={Boolean(formState.errors.firstName?.message)}
                      placeholder="Enter your First Name"
                      {...field}
                    />
                  </FormControl>
                  {formState.errors.firstName?.message ? (
                    <FormMessage />
                  ) : (
                    <FormDescription>e.g. Jonson</FormDescription>
                  )}
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="lastName"
              render={({ field, formState }) => (
                <FormItem className="">
                  <FormLabel>Last Name</FormLabel>
                  <FormControl>
                    <Input
                      isInvalid={Boolean(formState.errors.lastName?.message)}
                      placeholder="Enter your Last Name"
                      {...field}
                    />
                  </FormControl>
                  {formState.errors.lastName?.message ? (
                    <FormMessage />
                  ) : (
                    <FormDescription>e.g. Doe</FormDescription>
                  )}
                </FormItem>
              )}
            />
          </div>
        </FormFieldSet>
        <FormFieldSet className="flex flex-col">
          <FormField
            control={form.control}
            name="email"
            render={({ field, formState }) => (
              <FormItem className="mt-[1.5rem]">
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
            render={({ field, fieldState }) => (
              <FormItem className="mt-[1.5rem]">
                <FormLabel>Password</FormLabel>
                <FormControl>
                  <PasswordField
                    isInvalid={Boolean(fieldState.error)}
                    placeholder="Enter your Password"
                    showPassword={showPassword}
                    setShowPassword={setShowPassword}
                    {...field}
                  />
                </FormControl>
                <FormDescriptionCircled message="At least one uppercase letter required" />
                <FormDescriptionCircled message="At least one lowercase letter required" />
                <FormDescriptionCircled message="At least one number required" />
                <FormDescriptionCircled message="At least one character required" />
                <FormDescriptionCircled message="Password must be 8-16 characters long" />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="confirmPassword"
            render={({ field, fieldState }) => (
              <FormItem className="mt-[1.5rem]">
                <FormLabel>Confirm New Password</FormLabel>
                <FormControl>
                  <PasswordField
                    isInvalid={Boolean(fieldState.error?.message)}
                    placeholder="Confirm New Password"
                    showPassword={showPassword}
                    setShowPassword={setShowPassword}
                    {...field}
                  />
                </FormControl>
                <FormDescriptionCircled message="Confirm password must match new password" />
              </FormItem>
            )}
          />
        </FormFieldSet>
        <Button type="submit" className="mt-[1.5rem]" disabled={isLoading}>
          {isLoading ? (
            <span className="animate-spin border-2 border-t-transparent border-white w-5 h-5 rounded-full"></span>
          ) : (
            "Sign Up"
          )}
        </Button>
        <Text className="mt-[16px]" variant="caption">
          Already have an account?{" "}
          <CustomLink asChild>
            <Link
              to="/signin"
              className="fontset-link text-link-foreground underline"
            >
              Login
            </Link>
          </CustomLink>{" "}
          instead
        </Text>
      </form>
    </Form>
  );
}
