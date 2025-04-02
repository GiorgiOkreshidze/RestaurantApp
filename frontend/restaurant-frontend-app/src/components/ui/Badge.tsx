import * as React from "react";
import { Slot } from "@radix-ui/react-slot";
import { cn } from "@/lib/utils";
import { ReservationStatus } from "@/types";

export const Badge = ({
  className,
  status,
  asChild = false,
  ...props
}: React.ComponentProps<"span"> & {
  asChild?: boolean;
  status: ReservationStatus;
}) => {
  const Comp = asChild ? Slot : "span";

  return (
    <Comp
      data-slot="badge"
      className={cn(
        "fontset-caption inline-flex items-center justify-center rounded px-[0.75rem]",
        status === "Reserved" && "bg-orange-100",
        status === "In Progress" && "bg-blue-100",
        status === "Canceled" && "bg-red-100",
        status === "Finished" && "bg-green-100",
        status === "On Stop" && "bg-red-100",
        className
      )}
      {...props}
    />
  );
};
