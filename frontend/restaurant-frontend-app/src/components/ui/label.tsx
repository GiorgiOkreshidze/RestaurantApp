import * as LabelPrimitive from "@radix-ui/react-label";

import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

function Label({
  className,
  ...props
}: ComponentProps<typeof LabelPrimitive.Root>) {
  return (
    <LabelPrimitive.Root
      className={cn("fontset-bodyBold inline-flex items-center", className)}
      {...props}
    />
  );
}

export { Label };
