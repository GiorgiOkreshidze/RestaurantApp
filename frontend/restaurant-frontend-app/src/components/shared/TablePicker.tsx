import { WaiterReservationsFormState } from "@/app/slices/waiterReservationsSlice";
import { Select } from "./Select";
import { cn } from "@/lib/utils";

export const TablePicker = (props: TablePickerProps) => {
  const { className, tableList, table, setDate } = props;
  return (
    <Select
      items={tableList.map((table) => ({
        id: table.tableNumber,
        label: `Table ${table.tableNumber}`,
      }))}
      value={table}
      setValue={setDate}
      placeholder="Any table"
      className={cn("w-full", className)}
    />
  );
};

type TablePickerProps = Pick<
  WaiterReservationsFormState,
  "tableList" | "table"
> & {
  className?: string;
  setDate: (tableNumber: string) => void;
};
