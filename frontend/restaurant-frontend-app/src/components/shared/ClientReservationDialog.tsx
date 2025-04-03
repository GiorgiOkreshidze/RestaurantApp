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
import { dateObjToDateStringUI } from "@/utils/dateTime";
import { useClientReservationDialog } from "@/hooks/useClientReservationDialog";
import type {
  Reservation,
  ReservationDialogProps,
} from "@/types/reservation.types";
import { TIME_SLOTS } from "@/utils/constants";
import { useSelector } from "react-redux";
import { selectReservationCreatingLoading } from "@/app/slices/reservationsSlice";

export const ClientReservationDialog = ({
  className,
  children,
  ...props
}: ReservationDialogProps & {
  className?: string;
  children: ReactElement;
}) => {
  const [reservation, setReservation] = useState<Reservation>();
  const [isReservationDialogOpen, setIsReservationDialogOpen] = useState(false);
  const [isConfirmDialogOpen, setIsConfirmDialogOpen] = useState(false);
  const reservationCreatingLoading = useSelector(
    selectReservationCreatingLoading,
  );
  const onSuccessCallback = (reservation: Reservation) => {
    setReservation(reservation);
    setIsConfirmDialogOpen(true);
    setIsReservationDialogOpen(false);
  };
  const form = useClientReservationDialog({
    ...props,
    reservation,
    onSuccessCallback,
  });

  return (
    <>
      <Dialog
        open={isReservationDialogOpen}
        onOpenChange={setIsReservationDialogOpen}
      >
        <DialogTrigger className={className} asChild>
          {children}
        </DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="!fontset-h2">
              Make a Reservation
            </DialogTitle>
            <DialogDescription className="!fontset-body text-foreground">
              You are making a reservation at{" "}
              <b>
                {form.locationAddress}, Table {form.tableNumber}
              </b>{" "}
              for <b>{dateObjToDateStringUI(form.date)}</b>
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={form.onSubmit}>
            <Text variant="h3">Guests</Text>
            <Text variant="caption">Please specify the number of guests.</Text>
            <Text variant="caption">
              Table seating capacity: {form.maxGuests} people
            </Text>
            <GuestsNumber
              guests={form.guests}
              increase={form.increaseGuests}
              decrease={form.decreaseGuests}
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
                items={TIME_SLOTS}
                value={form.time}
                setValue={form.setTime}
                className="w-full mt-[1rem]"
                selectedDate={form.date}
                disablePastTimes
              />
            </div>
            <Button
              type="submit"
              variant="primary"
              size="xl"
              className="mt-[2.5rem] w-full"
            >
              {reservationCreatingLoading ? (
                <Spinner color="var(--color-white)" className="size-[1.5rem]" />
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
        guests={form.guests}
        date={form.date}
        time={form.time}
        locationAddress={form.locationAddress}
        tableNumber={form.tableNumber}
        setIsReservationDialogOpen={setIsReservationDialogOpen}
        onCancelReservation={form.onCancelReservation}
      />
    </>
  );
};
