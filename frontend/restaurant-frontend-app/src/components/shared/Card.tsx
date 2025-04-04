import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

export const Card = ({
  children,
  className,
  ...props
}: ComponentProps<"div">) => {
  return (
    <div
      className={cn(
        "rounded-card bg-card-background shadow-card p-[1.5rem]",
        className,
      )}
      {...props}
    >
      {children}
    </div>
  );
};
