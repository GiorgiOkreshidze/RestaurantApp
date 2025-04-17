export const CircleIcon = (props: Props) => {
  const { color = "var(--color-primary, #00AD0C)" } = props;
  return (
    <svg
      data-testid="CircleIcon"
      xmlns="http://www.w3.org/2000/svg"
      width={16}
      height={16}
      fill="none"
      {...props}
    >
      <title>Circle Icon</title>
      <rect width="100%" height="100%" fill={color} rx={8} />
    </svg>
  );
};

interface Props {
  color?: string;
}
