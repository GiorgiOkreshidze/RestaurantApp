import { useBookingFormStore } from "@/app/useBookingFormStore";
import {
  dateObjToDateStringServer,
  timeString24hToDateObj,
} from "@/utils/dateTime";
import { useEffect, type FormEvent } from "react";
import { useAppDispatch } from "../app/hooks";
import { fetchTables } from "@/app/thunks/tablesThunk";
import { isPast } from "date-fns";

export const useBookingForm = () => {
  const dispatch = useAppDispatch();
  const formStore = useBookingFormStore();
  const { locationId, date, guests, time, setTime } = formStore;

  useEffect(() => {
    if (!time) return;
    if (isPast(timeString24hToDateObj(time.split(" - ")[0]))) {
      setTime("");
    }
  }, [date]);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!locationId || !date) return;
    try {
      await dispatch(
        fetchTables({
          locationId,
          date: dateObjToDateStringServer(date),
          guests: String(guests),
          time: time.split("-")[0],
        }),
      );
    } catch (e) {
      console.error("Tables receiving failed", e);
    }
  };

  return { onSubmit, ...formStore };
};
