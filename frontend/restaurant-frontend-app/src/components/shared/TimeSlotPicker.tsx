import {
  SelectTrigger,
  SelectRoot,
  SelectContent,
  SelectValue,
  SelectItem,
} from "@/components/ui/SelectPrimitives";
import type { JSX } from "react";
import { Button, Spinner } from "../ui";
import { ChevronDownIcon, ClockIcon } from "../icons";
import { isToday } from "date-fns";
import { cn } from "@/lib/utils";
import type { RichTimeSlot } from "@/types";

export const TimeSlotPicker = ({
  items,
  className,
  value,
  setValue,
  loading,
  selectedDate,
}: {
  items: RichTimeSlot[];
  Icon?: () => JSX.Element;
  className?: string;
  value?: string | null;
  setValue: (value: string) => void;
  loading?: boolean;
  selectedDate: Date;
}) => {
  const handleChange = (id: string) => {
    setValue(id === "null" ? "" : id);
  };

  return (
    <SelectRoot value={value ?? ""} onValueChange={handleChange}>
      <SelectTrigger className={cn("w-full", className)} asChild>
        <Button variant="trigger" size="trigger">
          {<ClockIcon className="size-[1.5rem]" />}
          {loading ? (
            <span>Loading...</span>
          ) : (
            <SelectValue placeholder="Time" />
          )}
          {loading ? <Spinner className="size-[1em]" /> : <ChevronDownIcon />}
        </Button>
      </SelectTrigger>
      <SelectContent className="shadow-card [&>*]:tabular-nums [&>*]:font-sans">
        <SelectItem value="null">{"Time"}</SelectItem>
        {items?.map((timeSlot) => (
          <SelectItem
            key={timeSlot.rangeString}
            value={timeSlot.rangeString}
            disabled={isToday(selectedDate) && timeSlot.isPast}
          >
            {timeSlot.rangeString}
          </SelectItem>
        ))}
      </SelectContent>
    </SelectRoot>
  );
};
