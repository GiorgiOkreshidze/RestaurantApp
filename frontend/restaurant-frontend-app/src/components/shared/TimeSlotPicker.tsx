import {
  SelectTrigger,
  SelectRoot,
  SelectContent,
  SelectValue,
  SelectItem,
} from "@/components/ui/SelectPrimitives";
import { useEffect, type JSX } from "react";
import { Button, Spinner } from "../ui";
import { ChevronDownIcon, ClockIcon } from "../icons";
import { isPast, isToday } from "date-fns";
import { cn } from "@/lib/utils";
import type { RichTimeSlot } from "@/types";
import {
  timeString24hToDateObj,
  time24hTo12h,
} from "@/utils/dateTime";

export const TimeSlotPicker = (props: TimeSlotPickerProps) => {
  const {
    items,
    className,
    value,
    setValue,
    loading,
    selectedDate,
    disablePastTimes,
  } = props;

  const handleChange = (id: string) => {
    setValue(id === "null" ? "" : id);
  };

  useEffect(() => {
    if (
      disablePastTimes && isToday(selectedDate) && value
        ? isPast(timeString24hToDateObj(value?.split(" - ")[0]))
        : false
    ) {
      setValue("");
    }
  }, [props.selectedDate, disablePastTimes, selectedDate, setValue, value]);

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
            disabled={
              disablePastTimes && isToday(selectedDate) && timeSlot.isPast
            }
          >
            {time24hTo12h(timeSlot.startString)} -{" "}
            {time24hTo12h(timeSlot.endString)}
          </SelectItem>
        ))}
      </SelectContent>
    </SelectRoot>
  );
};

interface TimeSlotPickerProps {
  items: RichTimeSlot[];
  Icon?: () => JSX.Element;
  className?: string;
  value: string | null;
  setValue: (value: string) => void;
  loading?: boolean;
  selectedDate: string;
  disablePastTimes?: boolean;
}
