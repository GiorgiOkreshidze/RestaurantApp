import { ComponentPropsWithRef } from "react";

const Input = ({
  className,
  type = "text",
  ref,
  ...props
}: ComponentPropsWithRef<"input">) => {
  return (
    <input
      type={type}
      className={
        "font-[300] p-[1.14em] text-[0.875rem]/[1.5rem] bg-neutral-0 text-neutral-900 w-full border border-neutral-200 rounded placeholder:text-neutral-400"
      }
      ref={ref}
      {...props}
    />
  );
};

export { Input };
