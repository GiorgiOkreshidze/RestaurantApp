import { useWaiterReservations } from "@/hooks/useWaiterReservations";
import { DatePicker } from "./DatePicker";
import { TimeSlotPicker } from "./TimeSlotPicker";
import { TablePicker } from "./TablePicker";
import { Button } from "../ui";
import { SearchMagnifierIcon } from "../icons";

export const WaiterReservationsForm = () => {
  const form = useWaiterReservations();

  return (
    <form
      onSubmit={form.onSubmit}
      className="grid gap-[1rem] lg:justify-center lg:grid-cols-[repeat(3,minmax(max-content,270px))_auto]"
    >
      <DatePicker value={form.date} setValue={form.setFormDate} />

      <TimeSlotPicker
        items={form.timeList}
        value={form.time}
        setValue={form.setFormTime}
        selectedDate={form.date}
      />

      <TablePicker
        tableList={form.tableList}
        table={form.table}
        setDate={form.setFormTable}
      />

      <Button type="submit" variant="secondary" className="w-full">
        <SearchMagnifierIcon />
      </Button>
    </form>
  );
};
