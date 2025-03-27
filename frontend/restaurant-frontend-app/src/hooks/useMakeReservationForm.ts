import { useAppDispatch } from "@/app/hooks";
import { FormEvent, useState } from "react";
import { UseBookingForm } from "./useBookingForm";
import { dateObjectToYYYY_MM_DD } from "@/utils/dateTime";
import { Reservation, Table } from "@/types";
import {
  deleteClientReservation,
  upsertClientReservation,
} from "@/app/thunks/bookingThunk";
import { toast } from "react-toastify";

export const useMakeReservationForm = ({
  table,
  bookingForm,
  onSuccessCallback,
  reservation,
}: {
  table: Table;
  bookingForm: UseBookingForm;
  onSuccessCallback: (reservation: Reservation) => void;
  reservation: Reservation | null;
}) => {
  const dispatch = useAppDispatch();
  const [guestsNumber, setGuests] = useState(bookingForm.guests);
  const [time, setTime] = useState(bookingForm.time);

  const increaseGuestsNumber = () => {
    if (guestsNumber >= Number(table.capacity)) return;
    setGuests(guestsNumber + 1);
  };

  const decreaseGuestsNumber = () => {
    if (guestsNumber <= 0) return;
    setGuests(guestsNumber - 1);
  };

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!time) {
      toast.error("Select 'Time'");
      return;
    }
    try {
      const data = await dispatch(
        upsertClientReservation({
          ...(reservation?.id && { id: reservation.id }),
          locationId: bookingForm.locationId,
          date: dateObjectToYYYY_MM_DD(bookingForm.date),
          timeFrom: time.split("-")[0],
          timeTo: time.split("-")[1],
          tableNumber: table.tableNumber,
          guestsNumber: String(guestsNumber),
          tableId: table.tableId,
        }),
      ).unwrap();
      console.log("Reservation created");
      onSuccessCallback(data);
    } catch (error) {
      console.error("Reservation creating failed:", error);
    }
  };

  const onCancelReservation = async () => {
    if (!reservation) return;
    try {
      await dispatch(deleteClientReservation(reservation.id)).unwrap();
      console.log("Reservation deleted");
    } catch (error) {
      console.log("Reservation deleting failed:", error);
    }
  };

  return {
    guestsNumber,
    increaseGuestsNumber,
    decreaseGuestsNumber,
    time,
    setTime,
    onSubmit,
    onCancelReservation,
  };
};
