import { cva } from "class-variance-authority";

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
      buttonSecondary: "fontset-buttonSecondary",
      navigation: "fontset-navigation",
    },
  },
  defaultVariants: {
    variant: "body",
  },
});
