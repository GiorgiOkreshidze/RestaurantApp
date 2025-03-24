import { cn } from "@/lib/utils";
import { MinusIcon, PeopleIcon, PlusIcon } from "../icons";
import { Button, Text } from "../ui";

export const GuestsNumber = ({ className }: { className?: string }) => {
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
        <Button variant="secondary" size="sm" className="min-w-[40px]">
          <MinusIcon className="size-[1.5rem]" />
        </Button>
        <span>10</span>
        <Button variant="secondary" size="sm" className=" min-w-[40px]">
          <PlusIcon className="size-[1.5rem]" />
        </Button>
      </div>
    </div>
  );
};
