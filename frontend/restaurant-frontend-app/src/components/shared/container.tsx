import { cn } from "@/lib/utils";
import React from "react";

interface Props {
  className?: string;
}

export const Container: React.FC<React.PropsWithChildren<Props>> = ({
  className,
  children,
}) => {
  return (
    <div
      data-slot="container"
      className={cn(
        "flex *:basis-full min-w-[360px] w-full mx-auto max-w-[1440px] p-[20px] md:py-[24px] md:px-[40px]",
        className,
      )}
    >
      {children}
    </div>
  );
};
