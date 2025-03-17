import { cva, type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

const buttonVariants = cva(
  "inline-flex rounded min-w-[44px] cursor-pointer text-center w-full justify-center transition-all duration-300 items-center",
  {
    variants: {
      variant: {
        primary:
          "bg-primary text-neutral-0 fontset-buttonPrimary hover:bg-primary-dark active:bg-primary-darker disabled:bg-disabled",
        secondary:
          "bg-neutral-0 text-primary fontset-buttonPrimary hover:bg-primary-light active:bg-primary disabled:text-disabled",
        tertiary:
          "bg-transparent border-transparent text-foreground fontset-bodyBold hover:bg-transparent active:bg-primary-light text-disabled",
        outline:
          "border-[1px] border-solid border-green-200  bg-background shadow-sm hover:bg-accent hover:text-accent-foreground ",
      },
      size: {
        xl: "p-[1rem]",
        l: "py-[0.5rem] px-[1rem]",
        sm: "py-[0.25rem] px-[0.5rem]",
      },
    },
    defaultVariants: {
      variant: "primary",
      size: "xl",
    },
  }
);

function Button({
  className,
  variant,
  size,
  type = "button",
  ...props
}: ComponentProps<"button"> & VariantProps<typeof buttonVariants>) {
  return (
    <button
      data-slot="button"
      className={cn(buttonVariants({ variant, size, className }))}
      type={type}
      {...props}
    />
  );
}

export { Button, buttonVariants };
