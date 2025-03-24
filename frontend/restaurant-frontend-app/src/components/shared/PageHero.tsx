import { Container } from "./container";
import HeroImg from "../../assets/images/hero.jpg";
import { ComponentProps } from "react";
import { cn } from "@/lib/utils";
import { cva, VariantProps } from "class-variance-authority";

const heroVariants = cva("", {
  variants: {
    variant: {
      transparent: "py-[40px]",
      dark: "py-[98px] bg-[var(--color-neutral-900)]/80 bg-blend-overlay",
    },
  },
  defaultVariants: {
    variant: "transparent",
  },
});

export const PageHero = ({
  className,
  children,
  variant,
  ...props
}: ComponentProps<"div"> & VariantProps<typeof heroVariants>) => {
  return (
    <div
      className={cn(
        "content-center min-h-[404px] bg-cover bg-no-repeat bg-center",
        heroVariants({ variant, className }),
      )}
      style={{ backgroundImage: `url(${HeroImg})` }}
      {...props}
    >
      <Container>{children}</Container>
    </div>
  );
};
