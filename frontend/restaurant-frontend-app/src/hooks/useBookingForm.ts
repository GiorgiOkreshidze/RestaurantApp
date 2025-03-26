import { z } from "zod";
import { useAppDispatch } from "@/app/hooks";
import {
  clearTables,
  selectFilters,
  setFilters,
} from "@/app/slices/bookingSlice";
import { getTables } from "@/app/thunks/bookingThunk";
import {
  set,
  getDate,
  getMonth,
  getYear,
  getSeconds,
  getMilliseconds,
  getHours,
  getMinutes,
  startOfDay,
} from "date-fns";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { dateObjectToYYYY_MM_DD, dateObjectToHH_MM } from "@/utils/dateTime";

export const useBookingForm = () => {
  const dispatch = useAppDispatch();
  const filters = useSelector(selectFilters);
  const { locationId, dateTime, guests } = filters;
  const date = new Date(dateTime);

  const setLocationId = (locationId: string | null) => {
    dispatch(setFilters({ ...filters, locationId: locationId ?? "" }));
  };

  const setDate = (dateParam: Date | undefined) => {
    const newDate = set(dateTime, {
      year: getYear(dateParam ?? Date.now()),
      month: getMonth(dateParam ?? Date.now()),
      date: getDate(dateParam ?? Date.now()),
    }).toString();
    dispatch(setFilters({ ...filters, dateTime: newDate }));
  };

  const setTime = (dateParam: Date | undefined) => {
    const newDate = set(dateTime, {
      hours: getHours(dateParam ?? Date.now()),
      minutes: getMinutes(dateParam ?? Date.now()),
      seconds: getSeconds(dateParam ?? Date.now()),
      milliseconds: getMilliseconds(dateParam ?? Date.now()),
    }).toString();
    dispatch(setFilters({ ...filters, dateTime: newDate }));
  };

  const increaseGuestsNumber = () => {
    if (guests >= 10) return;
    dispatch(setFilters({ ...filters, guests: guests + 1 }));
  };
  const decreaseGuestsNumber = () => {
    if (guests <= 1) return;
    dispatch(setFilters({ ...filters, guests: guests - 1 }));
  };

  useEffect(() => {
    (async () => {
      if (!locationId) {
        dispatch(clearTables());
      }
    })();
  }, [locationId]);

  const formSchema = z.object({
    locationId: z.string().nonempty({ message: "Please select 'Location'" }),
    date: z.date().min(startOfDay(new Date()), {
      message: "Reservation date cannot be in the past",
    }),
    time: z.date(),
    guests: z.number(),
  });

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      locationId: locationId ?? "",
      date: date,
      time: date,
      guests: guests,
    },
    mode: "onSubmit",
    criteriaMode: "all",
  });

  const onSubmit = async (_: z.infer<typeof formSchema>) => {
    try {
      await dispatch(
        getTables({
          locationId: locationId,
          date: dateObjectToYYYY_MM_DD(date),
          time: dateObjectToHH_MM(date),
          guests: String(guests),
        }),
      ).unwrap();
      console.log("Form submited");
    } catch (error) {
      console.error("Registration failed:", error);
    }
  };

  return {
    locationId,
    setLocationId,
    date,
    setDate,
    setTime,
    guests,
    increaseGuestsNumber,
    decreaseGuestsNumber,
    onSubmit,
    form,
  };
};
