import { ComponentProps, ElementType } from "react";
import { cva, type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";

export const textVariants = cva("text-foreground", {
  variants: {
    variant: {
      h1: "fontset-h1",
      h2: "fontset-h2",
      h3: "fontset-h3",
      blockTitle: "fontset-blockTitle",
      body: "fontset-body",
      bodyBold: "fontset-bodyBold",
      caption: "fontset-caption",
      link: "fontset-link",
      buttonPrimary: "fontset-buttonPrimary",
    },
  },
  defaultVariants: {
    variant: "body",
  },
});

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
