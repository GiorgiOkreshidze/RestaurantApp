import { useAppDispatch } from "@/app/hooks";
import { selectLocationTables } from "@/app/slices/locationsSlice";
import {
  selectWaiterReservationsForm,
  setFormDateAction,
  setFormTableAction,
  setFormTimeAction,
} from "@/app/slices/waiterReservationsSlice";
import { getReservations } from "@/app/thunks/reservationsThunks";
import { formatDateToServer } from "@/utils/dateTime";
import { FormEvent } from "react";
import { useSelector } from "react-redux";

export const useWaiterReservations = () => {
  const dispatch = useAppDispatch();
  const formState = useSelector(selectWaiterReservationsForm);
  const locationTables = useSelector(selectLocationTables);

  const formActions = {
    setFormDate: (tableId: string) => dispatch(setFormDateAction(tableId)),
    setFormTime: (time: string) => dispatch(setFormTimeAction(time)),
    setFormTable: (tableNumber: string) =>
      dispatch(setFormTableAction(tableNumber)),
  };

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    try {
      await dispatch(
        getReservations({
          date: formState.date
            ? formatDateToServer(formState.date)
            : undefined,
          timeFrom: formState.time.split(" - ")[0] || undefined,
          tableNumber: locationTables.find(
            (table) => table.tableId === formState.table,
          )?.tableNumber,
        }),
      );
    } catch (e) {
      console.error("Tables receiving failed", e);
    }
  };

  return { ...formState, onSubmit, ...formActions };
};
