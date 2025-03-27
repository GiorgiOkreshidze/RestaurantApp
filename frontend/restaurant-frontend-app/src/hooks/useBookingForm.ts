import { useAppDispatch } from "@/app/hooks";
import {
  clearLocationTimeSlots,
  selectLocationTimeSlots,
  selectLocationTimeSlotsLoading,
  selectTodaySlots,
} from "@/app/slices/bookingSlice";
import {
  selectSelectOptions,
  selectSelectOptionsLoading,
} from "@/app/slices/locationsSlice";
import { getLocationTimeSlots, getTables } from "@/app/thunks/bookingThunk";
import { getSelectOptions } from "@/app/thunks/locationsThunks";
import { SelectOption } from "@/types";
import { dateObjectToYYYY_MM_DD } from "@/utils/dateTime";
import { startOfToday, startOfTomorrow } from "date-fns";
import { Dispatch, FormEvent, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { toast } from "react-toastify";

export const useBookingForm = (): UseBookingForm => {
  const [locationId, setLocationId] = useState("");
  const [guests, setGuests] = useState(2);
  const [time, setTime] = useState("");
  const dispatch = useAppDispatch();
  const selectOptions = useSelector(selectSelectOptions);
  const selectOptionsLoading = useSelector(selectSelectOptionsLoading);
  const locationTimeSlots = useSelector(selectLocationTimeSlots);
  const locationTimeSlotsLoading = useSelector(selectLocationTimeSlotsLoading);
  const todaySlots = useSelector(selectTodaySlots);
  const [date, setDate] = useState(
    todaySlots.length ? startOfToday() : startOfTomorrow(),
  );

  const increaseGuests = () => {
    if (guests >= 10) return;
    setGuests(guests + 1);
  };
  const decreaseGuests = () => {
    if (guests <= 1) return;
    setGuests(guests - 1);
  };

  useEffect(() => {
    (async () => {
      const data = await dispatch(getSelectOptions()).unwrap();
      if (data.length > 0) {
        setLocationId(data[0].id);
      }
    })();
  }, []);

  useEffect(() => {
    (async () => {
      dispatch(clearLocationTimeSlots());
      if (!locationId) return;
      await dispatch(
        getLocationTimeSlots({
          locationId,
          date: dateObjectToYYYY_MM_DD(date),
        }),
      ).unwrap();
    })();
  }, [locationId, date]);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!locationId) {
      toast.error("Select 'Location'");
      return;
    }
    try {
      await dispatch(
        getTables({
          locationId: locationId,
          date: dateObjectToYYYY_MM_DD(date),
          time: time.split("-")[0],
          guests: String(guests),
        }),
      ).unwrap();
      console.log("Tables recieved");
    } catch (error) {
      console.error("Tables recieving failed:", error);
    }
  };

  return {
    onSubmit,
    locationId,
    setLocationId,
    date,
    setDate,
    time,
    setTime,
    guests,
    increaseGuests,
    decreaseGuests,
    selectOptions,
    selectOptionsLoading,
    locationTimeSlots,
    locationTimeSlotsLoading,
  };
};

export interface UseBookingForm {
  onSubmit: (e: FormEvent) => Promise<void>;
  locationId: string;
  setLocationId: Dispatch<React.SetStateAction<string>>;
  date: Date;
  setDate: Dispatch<React.SetStateAction<Date>>;
  time: string;
  setTime: Dispatch<React.SetStateAction<string>>;
  guests: number;
  increaseGuests: () => void;
  decreaseGuests: () => void;
  selectOptions: SelectOption[];
  selectOptionsLoading: boolean;
  locationTimeSlots: string[];
  locationTimeSlotsLoading: boolean;
}
