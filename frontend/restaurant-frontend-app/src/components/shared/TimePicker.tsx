import { Button, Text } from "@/components/ui/";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/";
import { ChevronDownIcon, ClockIcon } from "../icons";
import { useRef, useState } from "react";
import { Label } from "@radix-ui/react-label";
import { TimePickerInput } from "@/components/ui/TimePicker/time-picker-input";
import { TimePeriodSelect } from "@/components/ui/TimePicker/time-picker-period";
import { Period } from "@/components/ui/TimePicker/time-picker-utils";
import { format } from "date-fns";

export function TimePicker({
  date,
  setDate,
}: {
  className?: string;
  date: Date;
  setDate: (date: Date | undefined) => void;
}) {
  const [period, setPeriod] = useState<Period>(format(date, "a") as Period);
  const minuteRef = useRef<HTMLInputElement>(null);
  const hourRef = useRef<HTMLInputElement>(null);
  const periodRef = useRef<HTMLButtonElement>(null);

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant="trigger" size="trigger">
          <ClockIcon className="size-[24px]" />
          <Text variant="buttonSecondary">{format(date, "h:mm aaaa")}</Text>
          <ChevronDownIcon className="text-foreground ml-auto" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-[1rem]">
        <div className="flex items-center gap-2">
          <div className="grid gap-[0.25rem] text-center">
            <Label htmlFor="hours" className="text-xs">
              Hours
            </Label>
            <TimePickerInput
              picker="12hours"
              period={period}
              date={date}
              setDate={setDate}
              ref={hourRef}
              onRightFocus={() => minuteRef.current?.focus()}
            />
          </div>
          <div className="grid gap-[0.25rem] text-center">
            <Label htmlFor="minutes" className="text-xs">
              Minutes
            </Label>
            <TimePickerInput
              picker="minutes"
              id="minutes12"
              date={date}
              setDate={setDate}
              ref={minuteRef}
              onLeftFocus={() => hourRef.current?.focus()}
              onRightFocus={() => periodRef.current?.focus()}
            />
          </div>
          <div className="grid gap-[0.25rem] text-center">
            <Label htmlFor="period" className="text-xs">
              Period
            </Label>
            <TimePeriodSelect
              period={period}
              setPeriod={setPeriod}
              date={date}
              setDate={setDate}
              ref={periodRef}
              onLeftFocus={() => minuteRef.current?.focus()}
            />
          </div>
        </div>
      </PopoverContent>
    </Popover>
  );
}
