import { ComponentProps } from "react";

export const CartIcon = (props: ComponentProps<"svg">) => (
  <svg
    xmlns="http://www.w3.org/2000/svg"
    viewBox="0 0 24 24"
    fill="none"
    {...props}
  >
    <path
      style={{ stroke: "var(--color-foreground, #232323)" }}
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={1.75}
      d="M8 22a1 1 0 1 0 0-2 1 1 0 0 0 0 2ZM19 22a1 1 0 1 0 0-2 1 1 0 0 0 0 2ZM2.05 2.05h2l2.66 12.42a2 2 0 0 0 2 1.58h9.78a2 2 0 0 0 1.95-1.57l1.65-7.43H5.12"
    />
  </svg>
);
