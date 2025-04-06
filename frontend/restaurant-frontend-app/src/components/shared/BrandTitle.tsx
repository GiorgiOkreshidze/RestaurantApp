import { cn } from "@/lib/utils";
import { cva, type VariantProps } from "class-variance-authority";
import { ComponentProps } from "react";

const brandTitleVariants = cva("", {
  variants: {
    variant: {
      huge: "font-[700] text-[clamp(2rem,5vw,5rem)]/[1]",
      navBarLogo: "font-[500] text-[1.5rem]/[1]",
      heroTitle: "fontset-h1 text-green-200",
    },
  },
});

export function BrandTitle({
  className,
  variant,
  ...props
}: ComponentProps<"p"> & Required<VariantProps<typeof brandTitleVariants>>) {
  return (
    <p className={cn(brandTitleVariants({ variant, className }))} {...props}>
      <span className="text-primary">Green</span>
      <span> & Tasty</span>
    </p>
  );
}
