import { cn } from "@/lib/utils";
import { cva, VariantProps } from "class-variance-authority";
import { ComponentProps } from "react";

const clockIconVariants = cva("", {
  variants: {
    variant: {
      primary: "stroke-primary",
      foreground: "stroke-foreground",
    },
  },
  defaultVariants: {
    variant: "primary",
  },
});

export const ClockIcon = ({
  className,
  variant,
  ...props
}: ComponentProps<"svg"> & VariantProps<typeof clockIconVariants>) => (
  <svg
    width={16}
    height={16}
    viewBox="0 0 16 16"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
    className={cn(clockIconVariants({ variant, className }))}
    {...props}
  >
    <g clipPath="url(#clip0_10183_13288)">
      <path
        d="M8.00004 14.6666C11.6819 14.6666 14.6667 11.6818 14.6667 7.99992C14.6667 4.31802 11.6819 1.33325 8.00004 1.33325C4.31814 1.33325 1.33337 4.31802 1.33337 7.99992C1.33337 11.6818 4.31814 14.6666 8.00004 14.6666Z"
        strokeWidth={1.16667}
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M8 4V8L10.6667 9.33333"
        strokeWidth={1.16667}
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </g>
    <defs>
      <clipPath id="clip0_10183_13288">
        <rect width={16} height={16} fill="white" />
      </clipPath>
    </defs>
  </svg>
);
