import {
  SelectTrigger,
  SelectRoot,
  SelectContent,
  SelectValue,
  SelectItem,
} from "@/components/ui/SelectPrimitives";
import { JSX } from "react";

export const Select = ({
  placeholder = "",
  items,
  renderIcon,
}: {
  items: {
    id: string;
    label: string;
  }[];
  placeholder?: string;
  renderIcon?: () => JSX.Element;
}) => {
  return (
    <SelectRoot>
      <SelectTrigger>
        {renderIcon && renderIcon()}
        <SelectValue placeholder={placeholder} />
      </SelectTrigger>
      <SelectContent>
        {items?.map((item, i) => (
          <SelectItem key={i} value={item.id}>
            {item.label}
          </SelectItem>
        ))}
      </SelectContent>
    </SelectRoot>
  );
};
