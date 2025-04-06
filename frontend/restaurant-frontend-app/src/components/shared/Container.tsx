import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

export const Container = ({ className, children }: ComponentProps<"div">) => {
  return (
    <div
      data-slot="container"
      className={cn("w-full mx-auto max-w-[1440px] px-[40px]", className)}
    >
      {children}
    </div>
  );
};
