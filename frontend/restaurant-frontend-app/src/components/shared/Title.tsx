import { Text } from "@/components/ui/";
import { cn } from "@/lib/utils";
import { cva, type VariantProps } from "class-variance-authority";
import { ComponentProps } from "react";

const titleVariants = cva("", {
  variants: {
    variant: {
      big: "font-[700]",
      small: "font-[500]",
    },
  },
  defaultVariants: {
    variant: "big",
  },
});

export function Title({
  className,
  variant,
  ...props
}: ComponentProps<"p"> & VariantProps<typeof titleVariants>) {
  return (
    <Text
      data-slot="title"
      className={cn(titleVariants({ variant, className }))}
      style={{ fontSize: "clamp(1rem, 6vw, 5rem)" }}
      {...props}
    >
      <span className="text-primary">Green</span>&nbsp;&&nbsp;Tasty
    </Text>
  );
}
