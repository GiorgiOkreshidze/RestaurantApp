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
  className,
}: {
  items: {
    id: string;
    label: string;
  }[];
  placeholder?: string;
  Icon?: ComponentType<{ className?: string }>;
  className?: string;
}) => {
  return (
    <SelectRoot>
      <SelectTrigger Icon={Icon} className={className}>
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
