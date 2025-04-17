import { useAppDispatch } from "@/app/hooks";
import {
  deleteClientReservation,
  getReservations,
  upsertClientReservation,
} from "@/app/thunks/reservationsThunks";
import { ClientReservationDialogProps } from "@/components/shared/ClientReservationDialog";
import { formatDateToServer } from "@/utils/dateTime";
import { type FormEvent, useEffect, useState } from "react";
import { toast } from "react-toastify";

export const useClientReservationDialog = (props: Props) => {
  const dispatch = useAppDispatch();
  const [reservationId, setReservationId] = useState(props.reservationId);
  const [selectedTime, setSelectedTime] = useState(props.initTime);
  const [selectedGuests, setSelectedGuests] = useState(props.initGuests);

  useEffect(() => {
    setSelectedGuests(props.initGuests);
  }, [props.initGuests]);

  const increaseGuests = () => {
    setSelectedGuests(
      selectedGuests < props.maxGuests ? selectedGuests + 1 : props.maxGuests,
    );
  };
  const decreaseGuests = () => {
    setSelectedGuests(selectedGuests > 1 ? selectedGuests - 1 : 1);
  };

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!selectedTime) {
      toast.error("Select 'Time'");
      return;
    }
    try {
      const data = await dispatch(
        upsertClientReservation({
          id: reservationId ?? undefined,
          locationId: props.locationId,
          date: formatDateToServer(props.date),
          timeFrom: selectedTime.split(" - ")[0],
          timeTo: selectedTime.split(" - ")[1],
          tableNumber: props.tableNumber,
          guestsNumber: String(selectedGuests),
          tableId: props.tableId,
        }),
      ).unwrap();
      setReservationId(data.id);
      props.onSuccessCallback();
      await dispatch(getReservations({}));
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
    try {
      if (!reservationId) throw new Error("There is no 'reservationId'");
      await dispatch(deleteClientReservation(reservationId)).unwrap();
      await dispatch(getReservations({}));
      console.log("Reservation deleted");
    } catch (error) {
      console.log("Reservation deleting failed:", error);
    }
  };

  return {
    onSubmit,
    selectedTime,
    setSelectedTime,
    increaseGuests,
    decreaseGuests,
    onCancelReservation,
    selectedGuests,
    setSelectedGuests,
    reservationId,
    setReservationId,
    locationAddress: props.locationAddress,
    tableNumber: props.tableNumber,
    date: props.date,
    maxGuests: props.maxGuests,
  };
};

interface Props extends ClientReservationDialogProps {
  id?: string;
  onSuccessCallback: () => void;
}
