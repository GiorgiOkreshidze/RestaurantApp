import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

export const ChevronDownIcon = ({
  className,
  ...props
}: ComponentProps<"svg">) => {
  return (
    <svg
      width={24}
      height={24}
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      className={cn("stroke-foreground size-[1.5rem]", className)}
      {...props}
    >
      <path
        d="M6 9l6 6 6-6"
        strokeWidth={1.75}
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};
