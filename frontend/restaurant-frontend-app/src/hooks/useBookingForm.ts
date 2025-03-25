import { useAppDispatch } from "@/app/hooks";
import { selectFilters, setFilters } from "@/app/slices/bookingSlice";
import { set, getDate, getMonth, getYear } from "date-fns";
import { FormEvent } from "react";
import { useSelector } from "react-redux";

export const useBookingForm = () => {
  const dispatch = useAppDispatch();
  const filters = useSelector(selectFilters);
  const { locationId, dateTime, guestsNumber } = useSelector(selectFilters);
  const date = new Date(dateTime);

  const setLocationId = (locationId: string | null) => {
    dispatch(setFilters({ ...filters, locationId }));
  };

  const setDate = (dateParam: Date | undefined) => {
    const newDate = set(dateTime, {
      year: getYear(dateParam ?? Date.now()),
      month: getMonth(dateParam ?? Date.now()),
      date: getDate(dateParam ?? Date.now()),
    }).toString();
    dispatch(setFilters({ ...filters, dateTime: newDate }));
  };

  const increaseGuestsNumber = () => {
    if (guestsNumber >= 10) return;
    dispatch(setFilters({ ...filters, guestsNumber: guestsNumber + 1 }));
  };
  const decreaseGuestsNumber = () => {
    if (guestsNumber <= 0) return;
    dispatch(setFilters({ ...filters, guestsNumber: guestsNumber - 1 }));
  };

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    try {
      // const result = await dispatch(register(values)).unwrap();
      console.log("Form submit");
      // navigate("/signin");
    } catch (error) {
      console.error("Registration failed:", error);
    }
  };

  return {
    locationId,
    setLocationId,
    date,
    setDate,
    guestsNumber,
    increaseGuestsNumber,
    decreaseGuestsNumber,
    onSubmit,
  };
};
