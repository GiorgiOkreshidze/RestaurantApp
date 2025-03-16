import { ComponentProps } from "react";
import { FormMessage } from "../ui";

const FormMessageCustom = ({ children, ...props }: ComponentProps<"p">) => {
  return (
    <FormMessage
      {...props}
      className="before:content-[''] before:w-[8px] before:h-[8px] before:rounded-full flex items-center gap-[0.5rem] before:bg-neutral-400 text-neutral-400"
    >
      {children}
    </FormMessage>
  );
};
