import { type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";
import type { ComponentProps, JSX } from "react";
import { Slot } from "@radix-ui/react-slot";
import { buttonVariants } from "@/components/variants/buttonVariants.ts";

export function Button({
  className,
  variant,
  size,
  icon,
  asChild = false,
  type = "button",
  children,
  ...props
}: ComponentProps<"button"> &
  VariantProps<typeof buttonVariants> & {
    asChild?: boolean;
    icon?: JSX.Element;
  }) {
  const Comp = asChild ? Slot : "button";

  return (
    <Comp
      data-slot="button"
      className={cn(buttonVariants({ variant, size, className }))}
      type={type}
      {...props}
    >
      <>
        {icon && <>{icon}</>}
        {children}
      </>
    </Comp>
  );
}
