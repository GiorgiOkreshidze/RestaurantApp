import { cva, type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";
import type { ComponentProps, JSX } from "react";
import { Slot } from "@radix-ui/react-slot";

export const buttonVariants = cva(
  "inline-flex rounded min-w-[44px] cursor-pointer text-center justify-center transition-all duration-300 items-center disabled:cursor-not-allowed border-[1px]",
  {
    variants: {
      variant: {
        primary:
          "bg-primary text-neutral-0 fontset-buttonPrimary hover:bg-primary-dark active:bg-primary-darker disabled:bg-disabled border-transparent",
        secondary:
          "bg-neutral-0 text-primary fontset-buttonPrimary hover:bg-primary-light active:bg-primary disabled:text-disabled disabled:bg-neutral-0 border-primary disabled:border-disabled",
        tertiary:
          "bg-transparent text-foreground fontset-bodyBold hover:bg-neutral-200 active:bg-primary-light disabled:text-disabled border-transparent disabled:bg-transparent",
        trigger:
          "grid grid-cols-[auto_1fr_auto] justify-start text-start bg-neutral-0 text-foreground fontset-buttonSecondary disabled:text-disabled disabled:bg-neutral-0 border-[1px] gap-[0.75rem] disabled:text-disabled disabled:bg-neutral-200 disabled:border-transparent border-neutral-200",
      },
      size: {
        xl: "p-[1rem]",
        l: "py-[0.5rem] px-[0.75rem]",
        sm: "py-[0.25rem] px-[0.5rem]",
        trigger: "py-[1rem] px-[1.5rem]",
      },
    },
    defaultVariants: {
      variant: "primary",
      size: "xl",
    },
  },
);

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
