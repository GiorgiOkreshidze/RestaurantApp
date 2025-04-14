import { ComponentProps, JSX } from "react";
import { ClockIcon } from "../icons";
import { Button, Text } from "../ui";
import { cn } from "@/lib/utils";
import { Link } from "react-router";
import { buttonVariants } from "@/components/variants/buttonVariants.ts";
import { useSelector } from "react-redux";
import { selectUser } from "@/app/slices/userSlice";

export const TimeSlot = ({
  children,
  className,
  icon,
  ...props
}: ComponentProps<"button"> & {
  icon?: JSX.Element;
}) => {
  const user = useSelector(selectUser);
  return (
    <>
      {user ? (
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
      ) : (
        <Link
          to="/signin"
          className={cn(
            buttonVariants({ variant: "secondary", size: "l" }),
            "flex justify-start gap-[0.5rem] whitespace-nowrap",
          )}
        >
          {icon ?? <ClockIcon className="size-[1rem] stroke-primary" />}
          <Text variant="bodyBold">{children}</Text>
        </Link>
      )}
    </>
  );
};
