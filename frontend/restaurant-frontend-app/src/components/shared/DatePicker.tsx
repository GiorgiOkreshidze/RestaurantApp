import { Button } from "@/components/ui/";
import { Calendar } from "@/components/ui/";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/";
import { CalendarIcon, ChevronDownIcon } from "../icons";
import { format } from "date-fns";
import { useState, type PropsWithChildren } from "react";

export function DatePicker({
  value,
  setValue,
  ...props
}: PropsWithChildren & {
  value: string;
  setValue: (date: string) => void;
}) {
  const [isOpened, setIsOpened] = useState(false);
  return (
    <Popover open={isOpened} onOpenChange={setIsOpened}>
      <PopoverTrigger asChild>
        <Button variant="trigger" size="trigger" className="w-full" {...props}>
          <CalendarIcon className="size-[1.5rem] w-full" />
          <span className="whitespace-nowrap">
            {format(value, "PP") || "Pick a date"}
          </span>
          <ChevronDownIcon />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={new Date(value)}
          onSelect={(date) =>
            setValue(date ? date.toString() : value.toString())
          }
          initialFocus
          disabled={{ before: new Date() }}
          onDayClick={() => setIsOpened(false)}
        />
      </PopoverContent>
    </Popover>
  );
}
