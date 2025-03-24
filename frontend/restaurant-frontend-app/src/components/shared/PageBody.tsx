import { cva, type VariantProps } from "class-variance-authority";
import { Container } from "./container";
import { ComponentProps } from "react";

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
