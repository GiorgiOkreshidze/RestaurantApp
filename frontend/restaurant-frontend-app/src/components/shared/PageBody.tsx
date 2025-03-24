import { cva, type VariantProps } from "class-variance-authority";
import { Container } from "./Container";
import { ComponentProps } from "react";
import { cn } from "@/lib/utils";

const PageBodyVariants = cva("", {
  variants: {
    variant: {
      biggerPadding: "pt-[4rem] pb-[2.5rem]",
      smallerPadding: "py-[2.5rem]",
    },
  },
  defaultVariants: {
    variant: "biggerPadding",
  },
});

export const PageBody = ({
  className,
  variant,
  children,
  ...props
}: ComponentProps<"div"> & VariantProps<typeof PageBodyVariants>) => {
  return (
    <Container className={PageBodyVariants({ variant, className })} {...props}>
      {children}
    </Container>
  );
};

export const PageBodySection = ({
  className,
  children,
  ...props
}: ComponentProps<"section">) => {
  return (
    <article className={cn("[&+&]:mt-[4rem]", className)} {...props}>
      {children}
    </article>
  );
};

export const PageBodyHeader = ({
  className,
  children,
  ...props
}: ComponentProps<"header">) => {
  return (
    <header className={cn("mb-[2.5rem]", className)} {...props}>
      {children}
    </header>
  );
};
