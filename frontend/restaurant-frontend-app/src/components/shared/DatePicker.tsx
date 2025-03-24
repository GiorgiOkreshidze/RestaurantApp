import { format } from "date-fns";
import { Calendar as CalendarIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/";
import { Calendar } from "@/components/ui/";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/";
import { ChevronDownIcon } from "../icons";
import { useState } from "react";

export function DatePicker({ className }: { className?: string }) {
  const [date, setDate] = useState<Date>();

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant="trigger"
          size="trigger"
          className={cn("", className, !date && "text-muted-foreground")}
        >
          <CalendarIcon className="size=[1.5rem]" />
          {date ? (
            <span className="whitespace-nowrap">{format(date, "PPP")}</span>
          ) : (
            <span className="whitespace-nowrap">Pick a date</span>
          )}
          <ChevronDownIcon />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={date}
          onSelect={setDate}
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );
}
