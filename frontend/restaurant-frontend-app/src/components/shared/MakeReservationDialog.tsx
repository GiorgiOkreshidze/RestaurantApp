import { Button, Text } from "../ui";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui";
import { ReactElement, useState } from "react";
import { Reservation, Table } from "@/types";
import { format, parse } from "date-fns";
import { GuestsNumber, TimeSlotPicker } from ".";
import { useMakeReservationForm } from "@/hooks/useMakeReservationForm";
import { UseBookingForm } from "@/hooks/useBookingForm";
import { useDispatch, useSelector } from "react-redux";
import { selectReservation } from "@/app/slices/bookingSlice";
import {
  convertDateToUIFormat,
  timeStringFrom24hTo12h,
} from "@/utils/dateTime";
import { useAppDispatch } from "@/app/hooks";

export const MakeReservationDialog = ({
  children,
  className,
  table,
  bookingForm,
  ownTimeSlot,
}: {
  children: ReactElement;
  className?: string;
  table: Table;
  bookingForm: UseBookingForm;
  ownTimeSlot: string;
}) => {
  const { locationAddress, tableNumber, capacity } = table;
  const { date, locationTimeSlots, locationTimeSlotsLoading } = bookingForm;
  const reservation = useSelector(selectReservation);
  const [createdReservation, setCreatedReservation] =
    useState<Reservation | null>(null);
  const [isCurrentDialogOpen, setIsCurrentDialogOpen] = useState(false);
  const [isSuccessDialogOpen, setIsSuccessDialogOpen] = useState(false);
  const {
    guestsNumber,
    increaseGuestsNumber,
    decreaseGuestsNumber,
    time,
    setTime,
    onSubmit,
    onCancelReservation,
  } = useMakeReservationForm({
    bookingForm,
    table,
    reservation,
    ownTimeSlot,
    onSuccessCallback: (reservation) => {
      setCreatedReservation(reservation);
      setIsSuccessDialogOpen(true);
      setIsCurrentDialogOpen(false);
    },
  });
  return (
    <>
      <Dialog open={isCurrentDialogOpen} onOpenChange={setIsCurrentDialogOpen}>
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
                {locationAddress}, Table {tableNumber}
              </b>{" "}
              for <b>{format(date, "PPP")}</b>
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={onSubmit}>
            <Text variant="h3">Guests</Text>
            <Text variant="caption">Please specify the number of guests.</Text>
            <Text variant="caption">
              Table seating capacity: {capacity} people
            </Text>
            <GuestsNumber
              guests={guestsNumber}
              increase={increaseGuestsNumber}
              decrease={decreaseGuestsNumber}
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
                items={locationTimeSlots}
                value={time}
                setValue={setTime}
                loading={locationTimeSlotsLoading}
                className="w-full mt-[1rem]"
                selectedDate={date}
              />
            </div>
            <Button
              type="submit"
              variant="primary"
              size="xl"
              className="mt-[2.5rem] w-full"
            >
              Make a Reservation
            </Button>
          </form>
        </DialogContent>
      </Dialog>

      {createdReservation && (
        <Dialog
          open={isSuccessDialogOpen}
          onOpenChange={setIsSuccessDialogOpen}
        >
          <DialogContent className="gap-[2.5rem]">
            <DialogHeader>
              <DialogTitle className="!fontset-h2">
                Reservation Confirmed!
              </DialogTitle>
            </DialogHeader>
            <div className="flex flex-col gap-[0.75rem]">
              <Text className="!fontset-body text-foreground">
                Your table reservation at <b>Green & Tasty</b> for{" "}
                <b>{createdReservation.guestsNumber} people</b> on{" "}
                <b>{convertDateToUIFormat(createdReservation.date)}</b>, from{" "}
                <b>{timeStringFrom24hTo12h(createdReservation.timeFrom)}</b> to{" "}
                <b>{timeStringFrom24hTo12h(createdReservation.timeTo)}</b> at{" "}
                <b>Table {createdReservation?.tableNumber}</b> has been
                successfully made.
              </Text>
              <Text className="!fontset-body text-foreground">
                We look forward to welcoming you at{" "}
                <b>{createdReservation.locationAddress}</b>.
              </Text>
              <Text className="!fontset-body text-foreground">
                If you need to modify or cancel your reservation, you can do so
                up to 30 min. before the reservation time.
              </Text>
            </div>
            <footer className="grid grid-cols-2 gap-[1rem]">
              <Button
                variant="secondary"
                size="l"
                onClick={onCancelReservation}
              >
                Cancel Reservation
              </Button>
              <Button
                variant="primary"
                size="l"
                onClick={() => {
                  setIsSuccessDialogOpen(false);
                  setIsCurrentDialogOpen(true);
                }}
              >
                Edit Reservation
              </Button>
            </footer>
          </DialogContent>
        </Dialog>
      )}
    </>
  );
};

// const timeSlots = [
//   "10:30 a.m. - 12:00 p.m",
//   "12:15 p.m. - 1:45 p.m",
//   "2:00 p.m. - 3:30 p.m",
//   "3:45 p.m. - 5:15 p.m",
//   "5:30 p.m. - 7:00 p.m",
//   "7:15 p.m. - 8:45 p.m",
//   "9:00 p.m. - 10:30 p.m",
// ];
