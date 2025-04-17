import { Container } from "./Container";
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
        "relative min-h-[404px] overflow-hidden ",
        heroVariants({ variant, className })
      )}
      {...props}
    >
      <div
        className={cn(
          "absolute inset-0 bg-cover bg-no-repeat bg-center z-0 scale-103",
          "sm:blur-none blur-[4px] transition-all duration-500"
        )}
        style={{ backgroundImage: `url(${HeroImg})` }}
      />

      <div className="relative z-10 content-center h-full">
        <Container>{children}</Container>
      </div>
    </div>
  );
};
