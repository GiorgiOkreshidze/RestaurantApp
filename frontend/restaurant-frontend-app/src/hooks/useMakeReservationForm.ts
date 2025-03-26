import { useState } from "react";

export const useMakeReservationForm = ({
  defaultGuests,
  maxCapacity,
  timeFrom,
}: { defaultGuests: number; maxCapacity: number; timeFrom: Date }) => {
  const [guestsNumber, setGuests] = useState(defaultGuests);

  const increaseGuestsNumber = () => {
    if (guestsNumber >= maxCapacity) return;
    setGuests(guestsNumber + 1);
  };

  const decreaseGuestsNumber = () => {
    if (guestsNumber <= 0) return;
    setGuests(guestsNumber - 1);
  };

  return { guestsNumber, increaseGuestsNumber, decreaseGuestsNumber };
};
