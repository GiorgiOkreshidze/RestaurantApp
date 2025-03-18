import { cn } from "@/lib/utils";
import { Slot } from "@radix-ui/react-slot";
import { ComponentProps } from "react";

export function CustomLink({
  children,
  className,
  asChild = false,
  ...props
}: ComponentProps<"a"> & { asChild?: boolean }) {
  const Comp = asChild ? Slot : "a";
  return (
    <Comp
      className={cn("fontset-link text-link-foreground underline", className)}
      {...props}
    >
      {children}
    </Comp>
  );
}
