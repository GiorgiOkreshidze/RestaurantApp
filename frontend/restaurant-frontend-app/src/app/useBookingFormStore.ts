import { useSelector } from "react-redux";
import {
  decreaseGuestsAction,
  increaseGuestsAction,
  selectBookingFormState,
  setDateAction,
  setLocationIdAction,
  setTimeAction,
} from "./slices/bookingFormSlice";
import { useAppDispatch } from "./hooks";

export const useBookingFormStore = () => {
  const dipatch = useAppDispatch();

  const formState = useSelector(selectBookingFormState);

  const formActions = {
    setLocationId: (locationId: string) =>
      dipatch(setLocationIdAction(locationId)),
    setDate: (date: Date) => dipatch(setDateAction(date)),
    setTime: (time: string) => dipatch(setTimeAction(time)),
    increaseGuests: () => dipatch(increaseGuestsAction()),
    decreaseGuests: () => dipatch(decreaseGuestsAction()),
  };

  return { ...formState, ...formActions };
};
