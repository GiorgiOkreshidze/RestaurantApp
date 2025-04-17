import { Button } from "@/components/ui/";
import { Calendar } from "@/components/ui/";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/";
import { CalendarIcon, ChevronDownIcon } from "../icons";
import { useState, type PropsWithChildren } from "react";
import { formatDateToUI } from "@/utils/dateTime";
import { cn } from "@/lib/utils";

export function DatePicker({
  value,
  setValue,
  className,
  ...props
}: PropsWithChildren & {
  value: string;
  setValue: (date: string) => void;
  className?: string;
}) {
  const [isOpened, setIsOpened] = useState(false);
  return (
    <Popover open={isOpened} onOpenChange={setIsOpened}>
      <PopoverTrigger asChild>
        <Button
          variant="trigger"
          size="trigger"
          className={cn("w-full", className)}
          {...props}
        >
          <CalendarIcon className="size-[1.5rem]" />
          <span className="whitespace-nowrap">
            {value ? formatDateToUI(value) : "Pick a date"}
          </span>
          <ChevronDownIcon />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={new Date(value)}
          onSelect={(date) => {
            if (!date) {
              setValue("");
            } else {
              setValue(date.toString());
            }
            setIsOpened(false);
          }}
          initialFocus
          disabled={{ before: new Date() }}
        />
      </PopoverContent>
    </Popover>
  );
}
