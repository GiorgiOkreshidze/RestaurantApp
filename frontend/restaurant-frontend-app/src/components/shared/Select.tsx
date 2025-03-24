import {
  SelectTrigger,
  SelectRoot,
  SelectContent,
  SelectValue,
  SelectItem,
} from "@/components/ui/SelectPrimitives";
import { ComponentType } from "react";

export const Select = ({
  placeholder,
  items,
  Icon,
}: {
  items: {
    id: string;
    label: string;
  }[];
  placeholder?: string;
  Icon?: ComponentType<{ className?: string }>;
}) => {
  return (
    <SelectRoot>
      <SelectTrigger Icon={Icon}>
        <SelectValue placeholder={placeholder ?? ""} />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="null">{placeholder}</SelectItem>
        {items?.map((item, i) => (
          <SelectItem key={i} value={item.id}>
            {item.label}
          </SelectItem>
        ))}
      </SelectContent>
    </SelectRoot>
  );
};
