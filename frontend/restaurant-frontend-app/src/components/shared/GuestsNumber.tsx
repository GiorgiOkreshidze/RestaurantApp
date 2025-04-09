import { cn } from "@/lib/utils";
import { MinusIcon, PeopleIcon, PlusIcon } from "../icons";
import { Button, Text } from "../ui";
import type { ComponentProps } from "react";
import { buttonVariants } from "@/components/variants/buttonVariants.ts";

export const GuestsNumber = ({
  guests,
  increase,
  decrease,
  className,
  ...props
}: ComponentProps<"div"> & {
  guests: number;
  increase: () => void;
  decrease: () => void;
}) => {
  return (
    <div
      className={cn(
        buttonVariants({ variant: "trigger", size: "trigger" }),
        "py-[0.75rem]",
        className,
      )}
      {...props}
    >
      <PeopleIcon className="size-[24px]" />
      <Text variant="buttonSecondary">Guests</Text>
      <div className="flex items-center gap-[0.5rem]">
        <Button
          variant="secondary"
          size="sm"
          className="min-w-[40px]"
          onClick={decrease}
        >
          <MinusIcon className="size-[1.5rem]" />
        </Button>
        <span className="min-w-[2ch] text-center fontset-buttonSecondary">
          {guests}
        </span>
        <Button
          variant="secondary"
          size="sm"
          className=" min-w-[40px]"
          onClick={increase}
        >
          <PlusIcon className="size-[1.5rem]" />
        </Button>
      </div>
    </div>
  );
};
