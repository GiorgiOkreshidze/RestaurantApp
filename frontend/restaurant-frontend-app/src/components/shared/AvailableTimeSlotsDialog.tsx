import { useBookingForm } from "@/hooks/useBookingForm";
import { ClockIcon } from "../icons";
import { Button } from "../ui";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui";
import { ReactElement } from "react";
import { Table } from "@/types";
import { timeStringFrom24hTo12h } from "@/utils/dateTime";
import { format } from "date-fns";

export const AvailableTimeSlotsDialog = ({
  children,
  className,
  table,
}: {
  children: ReactElement;
  className?: string;
  table: Table;
}) => {
  const { locationId, date } = useBookingForm();
  const { availableSlots, locationAddress, tableNumber } = table;
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
          {availableSlots.map((slot, i) => (
            <Button
              variant="secondary"
              size="sm"
              key={i}
              className="font-normal text-foreground flex gap-[0.5rem] justify-start fontset-bodyBold"
            >
              <ClockIcon />
              {timeStringFrom24hTo12h(slot.start)} -{" "}
              {timeStringFrom24hTo12h(slot.end)}
            </Button>
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
