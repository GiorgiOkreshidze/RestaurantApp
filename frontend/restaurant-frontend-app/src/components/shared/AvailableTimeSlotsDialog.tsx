import { ClockIcon } from "../icons";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui";
import { ReactElement } from "react";
import { timeString24hToTimeString12h } from "@/utils/dateTime";
import { format } from "date-fns";
import { MakeReservationDialog } from "./MakeReservationDialog";
import { TimeSlot } from "./TimeSlot";
import { UseBookingForm } from "@/hooks/useBookingForm";
import { TableUI } from "@/types/tables.types";

export const AvailableTimeSlotsDialog = ({
  children,
  className,
  table,
  date,
  bookingForm,
}: {
  children: ReactElement;
  className?: string;
  table: TableUI;
  date: Date;
  bookingForm: UseBookingForm;
}) => {
  const { availableSlots, locationAddress, tableNumber } = table;
  return (
    <Dialog>
      <DialogTrigger className={className} asChild>
        {children}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="!fontset-h2">Available slots</DialogTitle>
          <DialogDescription className="!fontset-body text-foreground">
            There are <b>{availableSlots.length} slots</b> available at{" "}
            <b>
              {locationAddress}, Table {tableNumber}
            </b>{" "}
            for <b>{format(date, "PPP")}</b>
          </DialogDescription>
        </DialogHeader>
        <div className="grid grid-cols-2 gap-[0.5rem] mt-[1rem]">
          {availableSlots.slice(0, 5).map((slot, i) => (
            <MakeReservationDialog
              table={table}
              key={i}
              bookingForm={bookingForm}
              ownTimeSlot={`${slot.start}-${slot.end}`}
            >
              <TimeSlot
                key={slot.start + slot.end}
                icon={<ClockIcon className="size-[1rem] stroke-primary" />}
              >
                {timeString24hToTimeString12h(slot.start)} -{" "}
                {timeString24hToTimeString12h(slot.end)}
              </TimeSlot>
            </MakeReservationDialog>
          ))}
        </div>
      </DialogContent>
    </Dialog>
  );
};

const timeSlots = [
  "10:30 a.m. - 12:00 p.m",
  "12:15 p.m. - 1:45 p.m",
  "2:00 p.m. - 3:30 p.m",
  "3:45 p.m. - 5:15 p.m",
  "5:30 p.m. - 7:00 p.m",
  "7:15 p.m. - 8:45 p.m",
  "9:00 p.m. - 10:30 p.m",
];
