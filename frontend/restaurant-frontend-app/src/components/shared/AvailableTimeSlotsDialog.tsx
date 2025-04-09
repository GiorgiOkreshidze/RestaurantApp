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
import { ClientReservationDialog } from "./ClientReservationDialog";
import { TimeSlot } from "./TimeSlot";
import { TableUI } from "@/types/tables.types";
import { RichTimeSlot } from "@/types";
import { useSelector } from "react-redux";
import { selectBooking } from "@/app/slices/bookingSlice";

export const AvailableTimeSlotsDialog = ({
  children,
  className,
  table,
  availableSlots,
}: {
  children: ReactElement;
  className?: string;
  table: TableUI;
  availableSlots: RichTimeSlot[];
}) => {
  const bookingForm = useSelector(selectBooking);
  return (
    <Dialog>
      <DialogTrigger className={className} asChild>
        {children}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="!fontset-h2">Available slots</DialogTitle>
          <DialogDescription className="!fontset-body text-foreground">
            There are <b>{table.availableSlots.length} slots</b> available at{" "}
            <b>
              {table.locationAddress}, Table {table.tableNumber}
            </b>{" "}
            for <b>{format(table.date, "PPP")}</b>
          </DialogDescription>
        </DialogHeader>
        <div className="grid grid-cols-2 gap-[0.5rem] mt-[1rem]">
          {availableSlots.map((timeSlot) => (
            <ClientReservationDialog
              reservationId={null}
              key={timeSlot.id}
              locationAddress={table.locationAddress}
              date={table.date}
              initTime={timeSlot.rangeString ?? bookingForm.time}
              tableNumber={table.tableNumber}
              initGuests={bookingForm.guests}
              maxGuests={Number.parseInt(table.capacity)}
              locationId={table.locationId}
              tableId={table.tableId}
            >
              <TimeSlot>
                {timeString24hToTimeString12h(timeSlot.startString)} -{" "}
                {timeString24hToTimeString12h(timeSlot.endString)}
              </TimeSlot>
            </ClientReservationDialog>
          ))}
        </div>
      </DialogContent>
    </Dialog>
  );
};
