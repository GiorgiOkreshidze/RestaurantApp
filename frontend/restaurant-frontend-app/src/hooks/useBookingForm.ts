import {
  dateObjToDateStringServer,
  timeString24hToDateObj,
} from "@/utils/dateTime";
import { useEffect, type FormEvent } from "react";
import { useAppDispatch } from "../app/hooks";
import { getTables } from "@/app/thunks/tablesThunk";
import { isPast, isToday } from "date-fns";
import { useSelector } from "react-redux";
import { toast } from "react-toastify";
import {
  decreaseGuestsAction,
  increaseGuestsAction,
  selectBookingFormState,
  setDateAction,
  setLocationAction,
  setTimeAction,
} from "@/app/slices/bookingFormSlice";

export const useBookingForm = () => {
  const dispatch = useAppDispatch();
  const formState = useSelector(selectBookingFormState);
  const formActions = {
    setLocation: (locationId: string) => {
      dispatch(setLocationAction(locationId));
    },
    setDate: (date: string) => {
      dispatch(setDateAction(date));
    },
    setTime: (time: string) => {
      dispatch(setTimeAction(time));
    },
    increaseGuests: () => {
      dispatch(increaseGuestsAction());
    },
    decreaseGuests: () => {
      dispatch(decreaseGuestsAction());
    },
  };

  const { locationId, date, guests, time } = formState;
  const { setTime } = formActions;

  useEffect(() => {
    if (!time || !isToday(date)) return;
    if (isPast(timeString24hToDateObj(time.split(" - ")[0]))) {
      setTime("");
    }
  }, [date]);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!locationId || !date) {
      toast.warning("Location and Date are required");
      return;
    }
    if (isPast(date) && isPast(timeString24hToDateObj(time.split(" - ")[0]))) {
      toast.warning("Date and Time can't be in past");
      return;
    }
    try {
      await dispatch(
        getTables({
          locationId,
          date: dateObjToDateStringServer(date),
          guests: String(guests),
          time: time.split(" - ")[0],
        }),
      );
    } catch (e) {
      console.error("Tables receiving failed", e);
    }
  };

  return { onSubmit, ...formState, ...formActions };
};
