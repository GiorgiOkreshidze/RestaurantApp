import { ComponentProps, ElementType } from "react";
import { cva, type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";

export const textVariants = cva("text-neutral-900", {
  variants: {
    variant: {
      h1: "text-[1.5rem]/[2.5rem] font-[500]",
      h2: "text-[1.5rem]/[2.5rem] font-[500]",
      h3: "text-[1.125rem]/[2rem] font-[500]",
      blockTitle: "text-[0.875rem]/[1.5rem] font-[300] uppercase",
      bodyBold: "text-[0.875rem]/[1.5rem] font-[500]",
      caption: "text-[0.75rem]/[1rem] font-[300] text-neutral-400",
      link: "text-[0.75rem]/[1rem] font-[700] text-blue-400",
      buttonPrimary: "text-[0.875rem]/[1.5rem] font-[700]",
      heroText: "text-[0.875rem]/[1.5rem] font-[300] ",
    },
  },
  defaultVariants: {
    // variant: "default",
  },
});

const Text = ({
  className,
  variant,
  as: Component = "p",
  ...props
}: ComponentProps<"p"> &
  VariantProps<typeof textVariants> & {
    as?: ElementType;
  }) => {
  return (
    <Component
      data-slot="text"
      className={cn(textVariants({ variant, className }))}
      {...props}
    />
  );
};

export { Text };
