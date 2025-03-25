import { format } from "date-fns";
import { Calendar as CalendarIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/";
import { Calendar } from "@/components/ui/";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/";
import { ChevronDownIcon } from "../icons";

export function DatePicker({
  className,
  value,
  setValue,
}: {
  className?: string;
  value: string;
  setValue: (value: string | null) => void;
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
          <span className="whitespace-nowrap">{value || "Pick a date"}</span>
          <ChevronDownIcon />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={value ? new Date(value) : undefined}
          onSelect={(date) => setValue(date ? format(date, "PP") : null)}
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );
}
