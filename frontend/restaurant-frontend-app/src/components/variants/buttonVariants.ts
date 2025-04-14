import { cva } from "class-variance-authority";

export const buttonVariants = cva(
  "inline-flex rounded min-w-[44px] cursor-pointer text-center justify-center transition-all duration-300 items-center disabled:cursor-not-allowed border-[1px]",
  {
    variants: {
      variant: {
        primary:
          "bg-primary text-neutral-0 fontset-buttonPrimary hover:bg-primary-dark active:bg-primary-darker disabled:bg-disabled border-transparent",
        secondary:
          "bg-neutral-0 text-primary fontset-buttonPrimary hover:bg-primary-light active:bg-primary disabled:text-disabled disabled:bg-neutral-0 border-primary disabled:border-disabled",
        tertiary:
          "bg-transparent text-foreground fontset-bodyBold hover:bg-neutral-200 active:bg-primary-light disabled:text-disabled border-transparent disabled:bg-transparent",
        underlined:
          "bg-transparent text-foreground fontset-bodyBold hover:bg-neutral-200 active:bg-primary-light disabled:text-disabled border-transparent disabled:bg-transparent relative before:absolute before:content[''] before:bottom-0 before:left-0 before:w-full before:h-[1px] before:bg-black disabled:before:bg-disabled @xs:mr-auto",
        trigger:
          "grid grid-cols-[auto_1fr_auto] justify-start text-start bg-neutral-0 text-foreground fontset-buttonSecondary disabled:text-disabled disabled:bg-neutral-0 border-[1px] gap-[0.75rem] disabled:text-disabled disabled:bg-neutral-50 border-neutral-200",
      },
      size: {
        xl: "p-[1rem]",
        l: "py-[0.5rem] px-[1rem]",
        sm: "py-[0.25rem] px-[0.5rem]",
        trigger: "py-[1rem] px-[1.5rem]",
      },
    },
    defaultVariants: {
      variant: "primary",
      size: "xl",
    },
  },
);
