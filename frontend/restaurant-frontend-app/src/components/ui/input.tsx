import { cn } from "@/lib/utils";
import { ComponentPropsWithRef } from "react";

const Input = ({
  className,
  type = "text",
  ref,
  isInvalid,
  ...props
}: ComponentPropsWithRef<"input"> & {
  isInvalid?: boolean;
}) => {
  return (
    <input
      type={type}
      className={cn(
        "fontset-body p-[1rem] bg-neutral-0 w-full border border-neutral-200 rounded placeholder:text-neutral-400 hover:shadow-input-primary hover:border-transparent focus:border-primary outline-none disabled:text-disabled m-0",
        isInvalid &&
          "border-destructive hover:border-destructive hover:shadow-input-destructive focus:border-destructive",
        className,
      )}
      ref={ref}
      {...props}
    />
  );
};

export { Input };
