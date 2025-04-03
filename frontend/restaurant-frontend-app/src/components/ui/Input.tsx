import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

export const Input = ({
  className,
  type = "text",
  isInvalid,
  ...props
}: ComponentProps<"input"> & {
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
      {...props}
    />
  );
};
