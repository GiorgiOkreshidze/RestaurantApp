import {
  SelectTrigger,
  SelectRoot,
  SelectContent,
  SelectValue,
  SelectItem,
} from "@/components/ui/SelectPrimitives";
import { ComponentType } from "react";
import { Button } from "../ui";
import { ChevronDownIcon, LocationIcon } from "../icons";

export const Select = ({
  placeholder,
  items,
  Icon,
  className,
  value,
  setValue,
}: {
  items: {
    id: string;
    label: string;
  }[];
  placeholder?: string;
  Icon?: ComponentType<{ className?: string }>;
  className?: string;
  value?: string | null;
  setValue: (value: string | null) => void;
}) => {
  const handleChange = (value: string) => {
    if (value === "null") {
      setValue("");
    } else {
      setValue(value);
    }
  };

  return (
    <SelectRoot value={value ?? ""} onValueChange={handleChange}>
      <SelectTrigger Icon={Icon} className={className} asChild>
        <Button variant="trigger" size="trigger">
          <LocationIcon className="shrink-0 stroke-foreground size-[1.5rem]" />
          <SelectValue placeholder={placeholder ?? ""} />
          <ChevronDownIcon />
        </Button>
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="null">Location</SelectItem>
        {items?.map((item, i) => (
          <SelectItem key={i} value={item.id}>
            {item.label}
          </SelectItem>
        ))}
      </SelectContent>
    </SelectRoot>
  );
};
