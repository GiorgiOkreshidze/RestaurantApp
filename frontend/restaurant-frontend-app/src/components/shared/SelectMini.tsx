import { SelectTrigger } from "@radix-ui/react-select";
import { SelectContent, SelectItem, SelectRoot } from "../ui/SelectPrimitives";
import { Button, Text } from "../ui";
import { cn } from "@/lib/utils";
import { ChevronDownIcon } from "lucide-react";

interface Props {
  className?: string;
  items: { id: string; label: string }[];
  value: string;
  placeholder: string;
  setValue: (value: string) => void;
}

export const SelectMini: React.FC<Props> = ({
  items,
  value,
  className,
  placeholder,
  setValue,
}) => {
  // Исправляем обработчик изменения, чтобы корректно устанавливать значение
  const handleChange = (newValue: string) => {
    setValue(newValue === "null" ? "" : newValue);
  };

  // Получаем текущий отображаемый текст (метку) для выбранного значения
  const getDisplayText = () => {
    if (!value) return placeholder;
    const selectedItem = items.find((item) => item.id === value);
    return selectedItem ? selectedItem.label : placeholder;
  };

  return (
    <SelectRoot value={value || "null"} onValueChange={handleChange}>
      <SelectTrigger className={cn("max-h-[32px] w-full", className)} asChild>
        <Button
          variant="trigger"
          className="h-[32px] py-1 px-2 w-full flex justify-between border-1 border-green-200 rounded-[8px]"
        >
          <Text variant="buttonPrimary" className="text-green-200">
            {getDisplayText()}
          </Text>
          <ChevronDownIcon />
        </Button>
      </SelectTrigger>
      <SelectContent>
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
