import { ComponentProps } from "react";

export const PlusIcon = (props: ComponentProps<"svg">) => (
  <svg
    width={16}
    height={16}
    viewBox="0 0 16 16"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
    {...props}
  >
    <path
      d="M3.3335 8H12.6668"
      style={{ stroke: "var(--color-primary, #00ad0c)" }}
      strokeWidth={1.16667}
      strokeLinecap="round"
      strokeLinejoin="round"
    />
    <path
      d="M8 3.33325V12.6666"
      style={{ stroke: "var(--color-primary, #00ad0c)" }}
      strokeWidth={1.16667}
      strokeLinecap="round"
      strokeLinejoin="round"
    />
  </svg>
);
