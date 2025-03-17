import { cn } from "@/lib/utils";
import { ComponentProps } from "react";

export const Container = ({ className, children }: ComponentProps<"div">) => {
  return (
    <div
      data-slot="container"
      className={cn(
        "min-w-[360px] w-full mx-auto max-w-[1440px] p-[20px] md:py-[24px] md:px-[40px]",
        className,
      )}
    >
      {children}
    </div>
  );
};
