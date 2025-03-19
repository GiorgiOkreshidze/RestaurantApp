import { ComponentProps } from "react";

export const PeopleIcon = (props: ComponentProps<"svg">) => (
  <svg
    width={16}
    height={16}
    viewBox="0 0 16 16"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
    {...props}
  >
    <path
      d="M12.0002 14.0001C12.0002 12.5856 11.4383 11.229 10.4381 10.2288C9.43787 9.22865 8.08132 8.66675 6.66683 8.66675C5.25234 8.66675 3.89579 9.22865 2.89559 10.2288C1.8954 11.229 1.3335 12.5856 1.3335 14.0001"
      style={{ stroke: "var(--color-primary, #00ad0c)" }}
      strokeWidth={1.16667}
      strokeLinecap="round"
      strokeLinejoin="round"
    />
    <path
      d="M6.66683 8.66667C8.50778 8.66667 10.0002 7.17428 10.0002 5.33333C10.0002 3.49238 8.50778 2 6.66683 2C4.82588 2 3.3335 3.49238 3.3335 5.33333C3.3335 7.17428 4.82588 8.66667 6.66683 8.66667Z"
      style={{ stroke: "var(--color-primary, #00ad0c)" }}
      strokeWidth={1.16667}
      strokeLinecap="round"
      strokeLinejoin="round"
    />
    <path
      d="M14.6669 13.3335C14.6669 11.0868 13.3335 9.00013 12.0002 8.00013C12.4385 7.67131 12.7889 7.23952 13.0206 6.74298C13.2522 6.24643 13.3579 5.70044 13.3282 5.15333C13.2985 4.60622 13.1345 4.07485 12.8505 3.60626C12.5666 3.13767 12.1715 2.7463 11.7002 2.4668"
      style={{ stroke: "var(--color-primary, #00ad0c)" }}
      strokeWidth={1.16667}
      strokeLinecap="round"
      strokeLinejoin="round"
    />
  </svg>
);
