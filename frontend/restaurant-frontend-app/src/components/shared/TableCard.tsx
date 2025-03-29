import type { TableUI } from "@/types/tables.types";
import { Text } from "../ui";
import { LocationIcon } from "../icons";
import { dateObjToDateStringUI } from "@/utils/dateTime";
import { ReservationDialog } from "./ReservationDialog";
import { useBookingFormStore } from "@/app/useBookingFormStore";

export const TableCard = ({ table }: { table: TableUI }) => {
  const bookingForm = useBookingFormStore();

  return (
    <li className="@container bg-card rounded overflow-hidden shadow-card">
      <article className="grid @max-[650px]:grid-rows-[200px_auto] @[650px]:grid-cols-[200px_1fr]">
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
              {table.availableSlots.length} slots available for{" "}
              {dateObjToDateStringUI(bookingForm.date)}:
            </Text>
          </div>
          <div className="grid gap-[0.5rem] @min-[400px]:grid-cols-2">
            {table.availableSlots.map((timeSlot) => (
              <ReservationDialog
                key={timeSlot.id}
                locationAddress={table.locationAddress}
                date={bookingForm.date}
                initTime={bookingForm.time}
                tableNumber={table.tableNumber}
                initGuests={bookingForm.guests}
                maxGuests={Number.parseInt(table.capacity)}
                locationId={table.locationId}
                tableId={table.tableId}
              >
                <p>Hello</p>
              </ReservationDialog>
            ))}
          </div>
        </div>
      </article>
    </li>
  );
};
