import { WaiterReservationsSearchForm } from "@/app/slices/waiterReservationsSlice";
import { Select } from "./Select";
import { cn } from "@/lib/utils";

export const TablePicker = (props: TablePickerProps) => {
  const { className, tableList, table, setTable } = props;
  return (
    <Select
      items={tableList.map((table) => ({
        id: table.tableId,
        label: `Table ${table.tableNumber}`,
      }))}
      value={table}
      setValue={setTable}
      placeholder="Any table"
      className={cn("w-full", className)}
    />
  );
};

type TablePickerProps = Pick<
  WaiterReservationsSearchForm,
  "tableList" | "table"
> & {
  className?: string;
  setTable: (tableNumber: string) => void;
};
