import {
  TooltipContent,
  TooltipProvider,
  TooltipRoot,
  TooltipTrigger,
} from "../ui";
import { ReactElement } from "react";

export const Tooltip = ({
  children,
  message,
  delayDuration,
  disabled,
  asChild,
}: {
  children: ReactElement;
  message: string;
  delayDuration?: number;
  disabled?: boolean;
  asChild?: boolean;
}) => {
  return (
    <TooltipProvider delayDuration={delayDuration}>
      <TooltipRoot open={disabled === true ? false : undefined}>
        <TooltipTrigger
          asChild={asChild}
          onPointerDownCapture={(e) => !disabled && e.stopPropagation()}
        >
          {children}
        </TooltipTrigger>
        <TooltipContent>{message}</TooltipContent>
      </TooltipRoot>
    </TooltipProvider>
  );
};
