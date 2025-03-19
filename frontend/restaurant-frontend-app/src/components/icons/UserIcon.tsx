import { ComponentProps } from "react";

export const UserIcon = (props: ComponentProps<"svg">) => (
  <svg
    xmlns="http://www.w3.org/2000/svg"
    viewBox="0 0 24 24"
    fill="none"
    {...props}
  >
    <path
      stroke="#232323"
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={1.75}
      d="M12 13a5 5 0 1 0 0-10 5 5 0 0 0 0 10Z"
    />
    <path
      stroke="#232323"
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={1.75}
      d="M20 21a8 8 0 0 0-16 0"
    />
  </svg>
);
