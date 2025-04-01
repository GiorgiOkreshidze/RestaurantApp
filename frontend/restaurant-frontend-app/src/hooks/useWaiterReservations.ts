import { useAppDispatch } from "@/app/hooks";
import {
  selectWaiterReservationsForm,
  setFormDateAction,
  setFormTableAction,
  setFormTimeAction,
} from "@/app/slices/waiterReservationsSlice";
import { FormEvent } from "react";
import { useSelector } from "react-redux";

export const useWaiterReservations = () => {
  const dispatch = useAppDispatch();

  const formStore = useSelector(selectWaiterReservationsForm);
  const formActions = {
    setFormDate: (tableId: string) => dispatch(setFormDateAction(tableId)),
    setFormTime: (time: string) => dispatch(setFormTimeAction(time)),
    setFormTable: (tableNumber: string) =>
      dispatch(setFormTableAction(tableNumber)),
  };

  const onSubmit = (e: FormEvent) => {
    e.preventDefault();
    console.log("send");
    try {
    } catch (e) {}
  };

  return { ...formStore, ...formActions, onSubmit };
};
