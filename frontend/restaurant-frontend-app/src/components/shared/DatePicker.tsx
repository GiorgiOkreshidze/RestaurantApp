import { format } from "date-fns";
import { Button } from "@/components/ui/";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Text } from "../ui";
import { CalendarIcon, ChevronDownIcon } from "../icons";

export const DatePicker = ({ value }: { value: string }) => {
  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant="trigger" size="trigger">
          <CalendarIcon className="stroke-foreground size-[1.5rem]" />
          <Text variant="buttonSecondary">
            {value ? format(value, "PPP") : "Date"}
          </Text>
          <ChevronDownIcon />
        </Button>
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
