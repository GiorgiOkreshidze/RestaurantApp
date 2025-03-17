import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

export function Link({ children, className, ...props }: ComponentProps<"a">) {
  return (
    <a
      className={cn("fontset-link text-link-foreground underline", className)}
      {...props}
    >
      {children}
    </a>
  );
}
