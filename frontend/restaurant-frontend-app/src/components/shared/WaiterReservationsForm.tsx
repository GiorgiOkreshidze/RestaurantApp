import { useWaiterReservations } from "@/hooks/useWaiterReservations";
import { DatePicker } from "./DatePicker";
import { TimeSlotPicker } from "./TimeSlotPicker";
import { TablePicker } from "./TablePicker";
import { Button } from "../ui";
import { SearchMagnifierIcon } from "../icons";
import { useRef } from "react";
import { useSelector } from "react-redux";
import { selectUser } from "@/app/slices/userSlice";
import { selectLocationTables } from "@/app/slices/locationsSlice";

export const WaiterReservationsForm = () => {
  const state = useWaiterReservations();
  const submitButton = useRef<HTMLButtonElement>(null);
  const waiter = useSelector(selectUser);
  const locationTables = useSelector(selectLocationTables);

  return (
    <form
      onSubmit={state.onSubmit}
      className="grid gap-[1rem] lg:justify-center lg:grid-cols-[repeat(3,minmax(max-content,270px))_auto]"
    >
      <DatePicker value={state.date} setValue={state.setFormDate} />

      <TimeSlotPicker
        items={state.timeList}
        value={state.time}
        setValue={state.setFormTime}
        selectedDate={state.date}
      />

      <TablePicker
        tableList={locationTables.filter(
          (table) => table.locationId === waiter?.locationId,
        )}
        table={state.table}
        setTable={state.setFormTable}
      />

      <Button
        type="submit"
        variant="secondary"
        className="w-full"
        ref={submitButton}
      >
        <SearchMagnifierIcon />
      </Button>
    </form>
  );
};
