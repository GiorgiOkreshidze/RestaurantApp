import {
  SelectTrigger,
  SelectRoot,
  SelectContent,
  SelectValue,
  SelectItem,
} from "@/components/ui/SelectPrimitives";
import type { JSX } from "react";
import { Button, Spinner } from "../ui";
import { ChevronDownIcon } from "../icons";

export const Select = ({
  placeholder,
  items,
  Icon,
  className,
  value,
  setValue,
  loading,
}: {
  items: {
    id: string;
    label: string;
  }[];
  placeholder?: string;
  Icon?: () => JSX.Element;
  className?: string;
  value?: string | null;
  setValue: (value: string) => void;
  loading?: boolean;
}) => {
  const handleChange = (id: string) => {
    setValue(id === "null" ? "" : id);
  };

  return (
    <SelectRoot value={value ?? ""} onValueChange={handleChange}>
      <SelectTrigger className={className} asChild>
        <Button variant="trigger" size="trigger">
          {Icon?.()}
          {loading ? (
            <span>Loading...</span>
          ) : (
            <SelectValue placeholder={placeholder ?? ""} />
          )}
          {loading ? <Spinner className="size-[1em]" /> : <ChevronDownIcon />}
        </Button>
      </SelectTrigger>
      <SelectContent className="shadow-card [&>*]:tabular-nums [&>*]:font-sans">
        <SelectItem value="null">{placeholder}</SelectItem>
        {items?.map((item) => (
          <SelectItem key={item.id} value={item.id}>
            {item.label}
          </SelectItem>
        ))}
      </SelectContent>
    </SelectRoot>
  );
};
