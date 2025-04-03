import { useAppDispatch } from "@/app/hooks";
import {
  deleteClientReservation,
  upsertClientReservation,
} from "@/app/thunks/reservationsThunks";
import type {
  Reservation,
  ReservationDialogProps,
} from "@/types/reservation.types";
import { dateObjToDateStringServer } from "@/utils/dateTime";
import { type FormEvent, useState } from "react";
import { toast } from "react-toastify";

export const useClientReservationDialog = ({
  onSuccessCallback,
  reservation,
  ...props
}: ReservationDialogProps & {
  onSuccessCallback: (reservation: Reservation) => void;
  reservation?: Reservation;
}) => {
  const maxGuests = props.maxGuests;
  const [time, setTime] = useState(reservation?.timeSlot ?? props.initTime);
  const [guests, setGuests] = useState(
    reservation?.guestsNumber
      ? Number.parseInt(reservation.guestsNumber)
      : props.initGuests,
  );
  const dispatch = useAppDispatch();
  const increaseGuests = () => {
    setGuests(guests < maxGuests ? guests + 1 : maxGuests);
  };

  const decreaseGuests = () => {
    setGuests(guests > 1 ? guests - 1 : 1);
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
          locationId: props.locationId,
          date: dateObjToDateStringServer(props.date),
          timeFrom: time.split("-")[0],
          timeTo: time.split("-")[1],
          tableNumber: props.tableNumber,
          guestsNumber: String(guests),
          tableId: props.tableId,
        }),
      ).unwrap();
      onSuccessCallback(data);
      console.log("Reservation created", data);
    } catch (error) {
      if (error instanceof Error) {
        console.error("Reservation creating failed:", error);
        toast.error(
          `Reservation creating failed: ${"message" in error ? error.message : ""}`,
        );
      }
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
    onSubmit,
    time,
    setTime,
    guests,
    increaseGuests,
    decreaseGuests,
    onCancelReservation,
    ...props,
  };
};
