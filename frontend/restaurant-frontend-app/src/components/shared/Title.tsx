import { cn } from "@/lib/utils";
import { cva, type VariantProps } from "class-variance-authority";
import { ComponentProps } from "react";

const titleVariants = cva("", {
  variants: {
    variant: {
      big: "font-[700] text-[clamp(2rem,5vw,5rem)]/[1]",
      navBarLogo: "font-[500]",
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
    <p className={cn(titleVariants({ variant, className }))} {...props}>
      <span className="text-primary">Green</span>
      <span> & Tasty</span>
    </p>
  );
}
