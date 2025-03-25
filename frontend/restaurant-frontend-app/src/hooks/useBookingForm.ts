import { useAppDispatch } from "@/app/hooks";
import { getTables } from "@/app/thunks/tablesThunk";
import { format } from "date-fns";
import { parseAsInteger, useQueryState } from "nuqs";
import { FormEvent, useEffect } from "react";

export const useBookingForm = () => {
  const [locationId, setLocationId] = useQueryState("locationId");
  const [date, setDate] = useQueryState("date", {
    defaultValue: format(Date.now(), "PP"),
  });
  const [guestsNumber, setGuestsNumber] = useQueryState(
    "guestsNumber",
    parseAsInteger.withDefault(2),
  );
  const dispatch = useAppDispatch();

  useEffect(() => {
    (async () => {
      if (!locationId) return;
      await dispatch(getTables({ locationId, date }));
    })();
  }, [locationId]);

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
    onSubmit,
    guestsNumber,
    setGuestsNumber,
  };
};
