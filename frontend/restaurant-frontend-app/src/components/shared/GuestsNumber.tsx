import { cn } from "@/lib/utils";
import { MinusIcon, PeopleIcon, PlusIcon } from "../icons";
import { Button, Text } from "../ui";
import { useBookingForm } from "@/hooks/useBookingForm";

export const GuestsNumber = () => {
  const { guestsNumber, increaseGuestsNumber, decreaseGuestsNumber } =
    useBookingForm();
  return (
    <div
      className={cn(
        "flex gap-[0.5rem] justify-between py-[0.75rem] px-[1.5rem] bg-card rounded",
      )}
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
          onClick={decreaseGuestsNumber}
        >
          <MinusIcon className="size-[1.5rem]" />
        </Button>
        <span className="min-w-[2ch] text-center">{guestsNumber}</span>
        <Button
          variant="secondary"
          size="sm"
          className=" min-w-[40px]"
          onClick={increaseGuestsNumber}
        >
          <PlusIcon className="size-[1.5rem]" />
        </Button>
      </div>
    </div>
  );
};
