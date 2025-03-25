import { ComponentProps, JSX } from "react";
import { ClockIcon } from "../icons";
import { Button, Text } from "../ui";
import { cn } from "@/lib/utils";

export const TimeSlot = ({
  children,
  className,
  icon,
  data,
  ...props
}: ComponentProps<"button"> & {
  icon?: JSX.Element;
  data: { start: string; end: string };
}) => {
  return (
    <Button
      variant="secondary"
      size="l"
      className={cn(
        "flex justify-start gap-[0.5rem] whitespace-nowrap",
        className,
      )}
      {...props}
    >
      {icon ?? <ClockIcon className="size-[1rem] stroke-primary" />}
      <Text variant="bodyBold">{children}</Text>
    </Button>
  );
};
