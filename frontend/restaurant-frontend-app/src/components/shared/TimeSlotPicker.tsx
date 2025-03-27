import {
  SelectTrigger,
  SelectRoot,
  SelectContent,
  SelectValue,
  SelectItem,
} from "@/components/ui/SelectPrimitives";
import { Dispatch, JSX, SetStateAction } from "react";
import { Button, Spinner } from "../ui";
import { ChevronDownIcon, ClockIcon } from "../icons";
import { isToday, parse } from "date-fns";

export const TimeSlotPicker = ({
  items,
  className,
  value,
  setValue,
  loading,
  selectedDate,
}: {
  items: string[];
  Icon?: () => JSX.Element;
  className?: string;
  value?: string | null;
  setValue: Dispatch<SetStateAction<string>>;
  loading?: boolean;
  selectedDate: Date;
}) => {
  const handleChange = (id: string) => {
    setValue(id === "null" ? "" : id);
  };

  const isPast = (item: string) => {
    const fromHH_MM = item.split("-")[0];
    const fromDate = parse(fromHH_MM, "HH:mm", new Date());
    return fromDate < new Date();
  };

  return (
    <SelectRoot value={value ?? ""} onValueChange={handleChange}>
      <SelectTrigger className={className} asChild>
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
        {items?.map((item, i) => (
          <SelectItem
            key={i}
            value={item}
            disabled={isToday(selectedDate) && isPast(item)}
          >
            {item}
          </SelectItem>
        ))}
      </SelectContent>
    </SelectRoot>
  );
};
