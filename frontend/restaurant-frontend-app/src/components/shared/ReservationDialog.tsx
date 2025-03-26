import { useBookingForm } from "@/hooks/useBookingForm";
import { Text } from "../ui";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui";
import { ReactElement, useState } from "react";
import { Table } from "@/types";
import { format } from "date-fns";
import { GuestsNumber, TimePicker } from ".";
import { useMakeReservationForm } from "@/hooks/useMakeReservationForm";

export const ReservationDialog = ({
  children,
  className,
  table,
}: {
  children: ReactElement;
  className?: string;
  table: Table;
}) => {
  const { locationId, date, guests } = useBookingForm();
  const { locationAddress, tableNumber, capacity } = table;
  const { guestsNumber, increaseGuestsNumber, decreaseGuestsNumber } =
    useMakeReservationForm({
      defaultGuests: guests,
      maxCapacity: Number(capacity),
      timeFrom: date,
    });
  return (
    <Dialog>
      <DialogTrigger
        className={className}
        asChild
        disabled={!locationId || !date}
      >
        {children}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="!fontset-h2">Make a Reservation</DialogTitle>
          <DialogDescription className="!fontset-body text-foreground">
            You are making a reservation at{" "}
            <b>
              {locationAddress}, Table {tableNumber}
            </b>{" "}
            for <b>{format(date, "PPP")}</b>
          </DialogDescription>
        </DialogHeader>
        <div>
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
            <div>
              <Text variant="buttonSecondary">From</Text>
              <TimePicker date={""} setDate={""} />
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
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
