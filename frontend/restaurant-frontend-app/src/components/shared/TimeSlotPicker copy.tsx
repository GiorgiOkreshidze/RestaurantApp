import { Dispatch, SetStateAction } from "react";
import { Select } from ".";
import { ClockIcon } from "../icons";

export const TimeSlotPicker = ({
  items,
  value,
  setValue,
  loading,
  ...props
}: {
  items: string[];
  value: string;
  setValue: Dispatch<SetStateAction<string>>;
  loading?: boolean;
  className?: string;
}) => {
  return (
    <Select
      Icon={() => <ClockIcon className="size-[1.5rem]" />}
      placeholder="Time"
      items={items?.map((item) => ({
        id: item,
        label: item,
      }))}
      value={value}
      setValue={setValue}
      loading={loading}
      {...props}
    />
  );
};
