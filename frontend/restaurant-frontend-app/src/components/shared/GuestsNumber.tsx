import { cn } from "@/lib/utils";
import { MinusIcon, PeopleIcon, PlusIcon } from "../icons";
import { Button, Text } from "../ui";

export const GuestsNumber = ({
  className,
  value,
  setValue,
}: {
  className?: string;
  value: number | null;
  setValue: (value: number) => void;
}) => {
  const handleIncrease = () => {
    if (value === null || value >= 10) return;
    setValue(value + 1);
  };

  const handleDecrease = () => {
    if (value === null || value <= 0) return;
    setValue(value - 1);
  };

  return (
    <div
      className={cn(
        "flex gap-[0.5rem] justify-between py-[0.75rem] px-[1.5rem] bg-card rounded",
        className,
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
          onClick={handleDecrease}
        >
          <MinusIcon className="size-[1.5rem]" />
        </Button>
        <span className="min-w-[2ch] text-center">{value}</span>
        <Button
          variant="secondary"
          size="sm"
          className=" min-w-[40px]"
          onClick={handleIncrease}
        >
          <PlusIcon className="size-[1.5rem]" />
        </Button>
      </div>
    </div>
  );
};
