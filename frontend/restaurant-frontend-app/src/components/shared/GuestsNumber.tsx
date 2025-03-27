import { cn } from "@/lib/utils";
import { MinusIcon, PeopleIcon, PlusIcon } from "../icons";
import { Button, Text } from "../ui";
import { ComponentProps } from "react";

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
        "styleSet-input flex gap-[0.5rem] justify-between py-[0.75rem] px-[1.5rem] bg-card rounded",
        className,
      )}
      {...props}
    >
      <div className="flex items-center gap-[0.5rem]">
        <PeopleIcon className="size-[24px]" />
        <Text variant="buttonSecondary">Guests</Text>
      </div>
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
