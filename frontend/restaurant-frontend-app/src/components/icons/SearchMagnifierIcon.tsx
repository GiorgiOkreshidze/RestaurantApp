import { ComponentProps } from "react";

export const SearchMagnifierIcon = (props: SearchMagnifierIcon) => {
  const { color = "var(--color-primary, #00ad0c)", ...restProps } = props;

  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      width={24}
      height={24}
      fill="none"
      {...restProps}
    >
      <title>Search Magnifier Icon</title>
      <path
        stroke={color}
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth={1.75}
        d="M11.5 19a8 8 0 1 0 0-16 8 8 0 0 0 0 16ZM21.5 21l-4.3-4.3"
      />
    </svg>
  );
};

type SearchMagnifierIcon = ComponentProps<"svg"> & { color?: string };
