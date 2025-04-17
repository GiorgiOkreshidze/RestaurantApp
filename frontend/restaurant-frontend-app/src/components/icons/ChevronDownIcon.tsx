import { ComponentProps } from "react";

export const ChevronDownIcon = (props: ComponentProps<"svg">) => {
  return (
    <svg
      data-testid="ChevronDownIcon"
      width={24}
      height={24}
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      {...props}
    >
      <title>Chevron Down Icon</title>
      <path
        d="M6 9l6 6 6-6"
        stroke="currentColor"
        strokeWidth={1.75}
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};
