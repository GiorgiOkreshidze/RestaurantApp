import { useBookingFormStore } from "@/app/useBookingFormStore";
import {
  dateObjToDateStringServer,
  timeString24hToDateObj,
} from "@/utils/dateTime";
import { useEffect, type FormEvent } from "react";
import { useAppDispatch } from "../app/hooks";
import { fetchTables } from "@/app/thunks/tablesThunk";
import { isPast } from "date-fns";
import { useSelector } from "react-redux";
import { selectSelectOptions } from "@/app/slices/locationsSlice";
import { toast } from "react-toastify";

export const useBookingForm = () => {
  const dispatch = useAppDispatch();
  const formStore = useBookingFormStore();
  const selectOptions = useSelector(selectSelectOptions);
  const { locationId, date, guests, time, setTime, setLocationId } = formStore;

  useEffect(() => {
    if (!selectOptions.length) return;
    setLocationId(selectOptions[0].id);
  }, [selectOptions, setLocationId]);

  useEffect(() => {
    if (!time) return;
    if (isPast(timeString24hToDateObj(time.split(" - ")[0]))) {
      setTime("");
    }
  }, [date, setTime, time]);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!locationId || !date) {
      toast.warning("Location and Date are required");
      return;
    }
    if (isPast(date) && isPast(timeString24hToDateObj(time.split("-")[0]))) {
      toast.warning("Date and Time can't be in past");
      return;
    }
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
