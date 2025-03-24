"use client";

import { format } from "date-fns";
import { CalendarIcon, ChevronDown } from "lucide-react";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/";
import { Calendar } from "@/components/ui/calendar";
import { FormControl } from "@/components/ui/Form";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Text } from "../ui";

export const DatePicker = ({ value }: { value: string }) => {
  return (
    <Popover>
      <PopoverTrigger asChild>
        <FormControl>
          <Button
            variant={"secondary"}
            className={cn(
              "flex items-center justify-between",
              value && "text-muted-foreground",
            )}
          >
            {value ? (
              format(value, "PPP")
            ) : (
              <div className="flex items-center gap-[0.5rem]">
                <CalendarIcon />
                <Text variant="buttonSecondary">Date</Text>
              </div>
            )}
            <ChevronDown />
          </Button>
        </FormControl>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0" align="start">
        <Calendar
          mode="single"
          selected={value}
          // onSelect={onChange}
          disabled={(date) =>
            date > new Date() || date < new Date("1900-01-01")
          }
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );
};
