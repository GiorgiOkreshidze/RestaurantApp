import { SortOptionType } from "@/types";
import { Text } from "../ui";
import { SelectMini } from "./SelectMini";

export const SortingOptions = ({
  options,
  value,
  onChange,
}: {
  options: SortOptionType[];
  value: string;
  onChange: (value: string) => void;
}) => (
  <div className="flex items-center gap-6" data-testid="sorting-options">
    <Text variant="bodyBold">Sort by:</Text>
    <div className="w-56">
      <SelectMini
        items={options}
        value={value}
        setValue={onChange}
        placeholder="Choose sorting"
      />
    </div>
  </div>
);
