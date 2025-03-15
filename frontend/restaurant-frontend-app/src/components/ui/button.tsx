import { Slot } from "@radix-ui/react-slot";
import { cva, type VariantProps } from "class-variance-authority";

import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

const buttonVariants = cva(
  "inline-flex rounded min-w-[44px] cursor-pointer text-center w-full justify-center transition-all duration-300",
  {
    variants: {
      variant: {
        primary:
          "bg-green-200 text-neutral-0 p-[1rem] hover:bg-green-300 active:bg-green-400 ",
        destructive:
          "bg-destructive text-white shadow-xs hover:bg-destructive/90 focus-visible:ring-destructive/20 dark:focus-visible:ring-destructive/40",
        outline:
          "border border-input border-green-200 bg-background shadow-xs text-green-200 hover:bg-accent hover:text-accent-foreground hover:bg-green-100 p-2 active:bg-green-200 active:text-neutral-0",
        secondary:
          "bg-secondary text-secondary-foreground shadow-xs hover:bg-secondary/80",
        ghost: "hover:bg-accent hover:text-accent-foreground",
        link: "text-primary underline-offset-4 hover:underline",
      },
      size: {
        default: "h-9 px-4 py-2 has-[>svg]:px-3",
        sm: "h-8 rounded-md gap-1.5 px-3 has-[>svg]:px-2.5",
        lg: "h-10 rounded-md px-6 has-[>svg]:px-4",
        icon: "size-9",
      },
    },
    defaultVariants: {
      variant: "primary",
      // size: "default",
    },
  }
);

function Button({
  className,
  variant,
  size,
  type = "button",
  asChild = false,
  ...props
}: ComponentProps<"button"> &
  VariantProps<typeof buttonVariants> & {
    asChild?: boolean;
  }) {
  const Comp = asChild ? Slot : "button";

  return (
    <Comp
      data-slot="button"
      className={cn(buttonVariants({ variant, size, className }))}
      type={type}
      {...props}
    />
  );
}

export { Button, buttonVariants };
