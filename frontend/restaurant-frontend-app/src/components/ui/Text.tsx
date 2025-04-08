import { ComponentProps, ElementType } from "react";
import { type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";
import { textVariants } from "../variants/textVariants";

const Text = ({
  className,
  variant,
  tag: Component = "p",
  ...props
}: ComponentProps<"p"> &
  VariantProps<typeof textVariants> & {
    tag?: ElementType;
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
