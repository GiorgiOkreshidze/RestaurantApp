import type { TableUI } from "@/types/tables.types";
import { Text } from "../ui";
import { LocationIcon } from "../icons";
import {
  dateObjToDateStringUI,
  timeString24hToTimeString12h,
} from "@/utils/dateTime";
import { ClientReservationDialog } from "./ClientReservationDialog";
import { TimeSlot } from "./TimeSlot";
import { isPast, isToday } from "date-fns";
import { AvailableTimeSlotsDialog } from "./AvailableTimeSlotsDialog";
import { selectBooking } from "@/app/slices/bookingSlice";
import { useSelector } from "react-redux";

export const TableCard = ({ table }: { table: TableUI }) => {
  const bookingForm = useSelector(selectBooking);
  const availableSlots = table.availableSlots.filter((timeSlot) => {
    return isToday(table.date) && isPast(timeSlot.startDate) ? false : true;
  });

  return (
    <li className="@container bg-card rounded overflow-hidden shadow-card">
      <article className="grid h-full @max-[650px]:grid-rows-[200px_auto] @[650px]:grid-cols-[200px_1fr]">
        <div>
          <img
            className="block object-cover w-full h-full"
            src={"/location-thumbnail.jpg"}
            alt={`Table ${table.tableNumber}`}
          />
        </div>
        <div className="@container p-[1.5rem] flex flex-col gap-[0.75rem]">
          <div className="flex flex-col gap-[1rem]">
            <Text variant="bodyBold" className="flex items-center gap-[0.5rem]">
              <LocationIcon className="size-[16px]" />
              <span>{table.locationAddress}</span>
              <span className="ml-auto">Table {table.tableNumber}</span>
            </Text>
            <Text variant="bodyBold">
              Table seating capacity: {table.capacity} people
            </Text>
            <Text variant="bodyBold">
              {availableSlots.length} slots available for{" "}
              {dateObjToDateStringUI(table.date)}:
            </Text>
          </div>
          <div className="grid gap-[0.5rem] @min-[400px]:grid-cols-2">
            {availableSlots.slice(0, 5).map((timeSlot) => (
              <ClientReservationDialog
                key={timeSlot.id}
                reservationId={null}
                locationId={table.locationId}
                locationAddress={table.locationAddress}
                tableId={table.tableId}
                tableNumber={table.tableNumber}
                date={table.date}
                initTime={timeSlot.rangeString}
                initGuests={bookingForm.guests}
                maxGuests={Number.parseInt(table.capacity)}
              >
                <TimeSlot>
                  {timeString24hToTimeString12h(timeSlot.startString)} -{" "}
                  {timeString24hToTimeString12h(timeSlot.endString)}
                </TimeSlot>
              </ClientReservationDialog>
            ))}
            {availableSlots.length > 5 ? (
              <AvailableTimeSlotsDialog
                table={table}
                availableSlots={availableSlots}
              >
                <TimeSlot>Show all</TimeSlot>
              </AvailableTimeSlotsDialog>
            ) : null}
          </div>
        </div>
      </article>
    </li>
  );
};
