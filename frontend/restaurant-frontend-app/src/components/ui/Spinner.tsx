import type { ComponentProps } from "react";

export const Spinner = ({
  color = "var(--color-primary, #00ad0c)",
  ...props
}: ComponentProps<"svg"> & {
  color?: string;
}) => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 200 200"
      width="50"
      height="50"
      {...props}
    >
      <title>Spinner</title>
      <radialGradient
        id="a12"
        cx={0.66}
        fx={0.66}
        cy={0.3125}
        fy={0.3125}
        gradientTransform="scale(1.5)"
      >
        <stop offset={0} stopColor={color} />
        <stop offset={0.3} stopColor={color} stopOpacity={0.9} />
        <stop offset={0.6} stopColor={color} stopOpacity={0.6} />
        <stop offset={0.8} stopColor={color} stopOpacity={0.3} />
        <stop offset={1} stopColor={color} stopOpacity={0} />
      </radialGradient>
      <circle
        style={{ transformOrigin: "center" }}
        fill="none"
        stroke={color}
        strokeWidth={15}
        strokeLinecap="round"
        strokeDasharray="200 1000"
        strokeDashoffset={0}
        cx={100}
        cy={100}
        r={70}
      >
        <animateTransform
          type="rotate"
          attributeName="transform"
          calcMode="spline"
          dur={1}
          values="360;0"
          keyTimes="0;1"
          keySplines="0 0 1 1"
          repeatCount="indefinite"
        />
      </circle>
      <circle
        style={{ transformOrigin: "center" }}
        fill="none"
        opacity={0.2}
        stroke={color}
        strokeWidth={15}
        strokeLinecap="round"
        cx={100}
        cy={100}
        r={70}
      />
    </svg>
  );
};
