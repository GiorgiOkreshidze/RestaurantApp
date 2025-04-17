import * as LabelPrimitive from "@radix-ui/react-label";
import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

export const Label = ({
  className,
  ...props
}: ComponentProps<typeof LabelPrimitive.Root>) => {
  return (
    <LabelPrimitive.Root
      data-testId="label"
      className={cn("fontset-bodyBold inline-flex items-center", className)}
      {...props}
    />
  );
};
