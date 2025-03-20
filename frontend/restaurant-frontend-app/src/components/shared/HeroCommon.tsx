import { Container } from "./container";
import HeroImg from "../../assets/images/hero.jpg";
import { ComponentProps } from "react";
import { cn } from "@/lib/utils";
import { cva, VariantProps } from "class-variance-authority";

const heroVariants = cva("", {
  variants: {
    variant: {
      transparent: "",
      dark: "bg-[var(--color-neutral-900)]/80 bg-blend-overlay",
    },
  },
  defaultVariants: {
    variant: "transparent",
  },
});

export const HeroCommon = ({
  className,
  children,
  variant,
  ...props
}: ComponentProps<"div"> & VariantProps<typeof heroVariants>) => {
  return (
    <div
      className={cn(
        "min-h-[400px] py-[100px] bg-cover bg-no-repeat bg-center",
        heroVariants({ variant, className }),
      )}
      style={{ backgroundImage: `url(${HeroImg})` }}
      {...props}
    >
      <Container>{children}</Container>
    </div>
  );
};
