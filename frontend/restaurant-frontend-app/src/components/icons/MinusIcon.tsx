import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

export const MinusIcon = ({ className, ...props }: ComponentProps<"svg">) => (
  <svg
    width="24px"
    height="24px"
    viewBox="0 0 24 24"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
    className={cn("stroke-primary", className)}
    {...props}
  >
    <path
      d="M5 12H19"
      strokeWidth={1.75}
      strokeLinecap="round"
      strokeLinejoin="round"
    />
  </svg>
);
