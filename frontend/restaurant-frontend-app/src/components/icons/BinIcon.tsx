import { ComponentProps } from "react";

export const BinIcon = (props:ComponentProps<"svg">) => (
  <svg
    xmlns="http://www.w3.org/2000/svg"
    width={25}
    height={24}
    fill="none"
    viewBox="0 0 25 24"
    {...props}
  >
    <path
      stroke="currentColor"
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={1.75}
      d="M3.5 6h18M19.5 6v14c0 1-1 2-2 2h-10c-1 0-2-1-2-2V6M8.5 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2M10.5 11v6M14.5 11v6"
    />
  </svg>
);
