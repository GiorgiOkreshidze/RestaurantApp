import { type ReactElement, useState } from "react";
import { GuestsNumber, ReservationConfirmDialog, TimeSlotPicker } from ".";
import {
  Button,
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  Spinner,
  Text,
} from "../ui";
import { formatDateToUI } from "@/utils/dateTime";
import { useClientReservationDialog } from "@/hooks/useClientReservationDialog";
import { useSelector } from "react-redux";
import { selectReservationCreatingLoading } from "@/app/slices/reservationsSlice";
import { selectBooking } from "@/app/slices/bookingSlice";

export const ClientReservationDialog = (
  props: ClientReservationDialogProps,
) => {
  const [isCurrentDialogOpen, setIsCurrentDialogOpen] = useState(false);
  const [isConfirmDialogOpen, setIsConfirmDialogOpen] = useState(false);
  const creatingLoading = useSelector(selectReservationCreatingLoading);
  const onSuccessCallback = () => {
    setIsCurrentDialogOpen(!isCurrentDialogOpen);
    setIsConfirmDialogOpen(!isConfirmDialogOpen);
  };
  const state = useClientReservationDialog({ ...props, onSuccessCallback });
  const booking = useSelector(selectBooking);

  return (
    <>
      <Dialog open={isCurrentDialogOpen} onOpenChange={setIsCurrentDialogOpen}>
        <DialogTrigger asChild>{props.children}</DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="!fontset-h2">
              {state.reservationId
                ? "Edit the Reservation"
                : "Make a Reservation"}
            </DialogTitle>
            <DialogDescription className="!fontset-body text-foreground">
              You are making a reservation at{" "}
              <b>
                {state.locationAddress}, Table {state.tableNumber}
              </b>{" "}
              for <b>{formatDateToUI(state.date)}</b>
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={state.onSubmit}>
            <Text variant="h3">Guests</Text>
            <Text variant="caption">Please specify the number of guests.</Text>
            <Text variant="caption">
              Table seating capacity: {state.maxGuests} people
            </Text>
            <GuestsNumber
              guests={state.selectedGuests}
              increase={state.increaseGuests}
              decrease={state.decreaseGuests}
              className="mt-[1.5rem]"
            />
            <Text variant="h3" className="mt-[2rem]">
              Time
            </Text>
            <Text variant="caption">
              Please choose your preferred time from the dropdowns below
            </Text>
            <div>
              <TimeSlotPicker
                items={booking.timeSlots}
                value={state.selectedTime}
                setValue={state.setSelectedTime}
                className="w-full mt-[1rem]"
                selectedDate={state.date}
                disablePastTimes
              />
            </div>
            <Button
              type="submit"
              variant="primary"
              size="xl"
              className="mt-[2.5rem] w-full"
            >
              {creatingLoading ? (
                <Spinner color="var(--color-white)" className="size-[1.5rem]" />
              ) : state.reservationId ? (
                "Edit reservation"
              ) : (
                "Make a Reservation"
              )}
            </Button>
          </form>
        </DialogContent>
      </Dialog>
      <ReservationConfirmDialog
        open={isConfirmDialogOpen}
        onOpenChange={setIsConfirmDialogOpen}
        guests={state.selectedGuests}
        date={state.date}
        time={state.selectedTime}
        locationAddress={state.locationAddress}
        tableNumber={state.tableNumber}
        setIsReservationDialogOpen={setIsCurrentDialogOpen}
        onCancelReservation={state.onCancelReservation}
      />
    </>
  );
};

export interface ClientReservationDialogProps {
  children: ReactElement;
  reservationId: string | null;
  locationId: string;
  locationAddress: string;
  tableId: string;
  tableNumber: string;
  date: string;
  initTime: string;
  initGuests: number;
  maxGuests: number;
}
