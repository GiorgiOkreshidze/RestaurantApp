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
        "w-full styleSet-input",
        isInvalid && "styleSet-input-invalid",
        className,
      )}
      ref={ref}
      {...props}
    />
  );
};

export { Input };
