import { Calendar as CalendarIcon } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/";
import { Calendar } from "@/components/ui/";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/";
import { ChevronDownIcon } from "../icons";
import { format } from "date-fns";

export function DatePicker({
  className,
  value,
  setValue,
}: {
  className?: string;
  value: Date;
  setValue: (date?: Date) => void;
}) {
  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant="trigger"
          size="trigger"
          className={cn("", className, !value && "text-muted-foreground")}
        >
          <CalendarIcon className="size=[1.5rem]" />
          <span className="whitespace-nowrap">
            {format(value, "PP") || "Pick a date"}
          </span>
          <ChevronDownIcon />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={value}
          onSelect={(date) => setValue(date)}
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );
}
