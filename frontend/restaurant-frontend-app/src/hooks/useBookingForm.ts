import { format } from "date-fns";
import { parseAsInteger, useQueryState } from "nuqs";
import { useEffect } from "react";

export const useBookingForm = () => {
  const [locationId, setLocationId] = useQueryState("locationId");
  const [date, setDate] = useQueryState("date", {
    defaultValue: format(Date.now(), "PPP"),
  });
  const [guestsNumber, setGuestsNumber] = useQueryState(
    "guestsNumber",
    parseAsInteger.withDefault(2),
  );

  useEffect(() => {}, [locationId, date]);

  const onSubmit = async () => {
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
    onSubmit,
    guestsNumber,
    setGuestsNumber,
  };
};
