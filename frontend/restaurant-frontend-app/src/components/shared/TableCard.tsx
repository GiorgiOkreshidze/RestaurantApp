import { Table } from "@/types";
import locationThumbnail from "../../assets/images/location-thumbnail.jpg";
import { ClockIcon, LocationIcon, PlusIcon } from "../icons";
import { Text } from "../ui";
import { TimeSlot } from "./TimeSlot";
import { format } from "date-fns";
import { AvailableTimeSlotsDialog } from "./AvailableTimeSlotsDialog";
import { MakeReservationDialog } from "./MakeReservationDialog";
import { timeStringFrom24hTo12h } from "@/utils/dateTime";
import { UseBookingForm } from "@/hooks/useBookingForm";

export const TableCard = ({
  table,
  bookingForm,
}: { table: Table; bookingForm: UseBookingForm }) => {
  const { availableSlots, locationAddress, capacity, tableNumber } = table;
  const { date } = bookingForm;
  return (
    <li className="@container bg-card rounded overflow-hidden shadow-card">
      <article className="grid @max-[650px]:grid-rows-[200px_auto] @[650px]:grid-cols-[200px_1fr]">
        <div className="">
          <img
            className="block object-cover w-full h-full"
            src={locationThumbnail}
          />
        </div>
        <div className="@container p-[1.5rem] flex flex-col gap-[0.75rem]">
          <div className="flex flex-col gap-[1rem]">
            <Text variant="bodyBold" className="flex items-center gap-[0.5rem]">
              <LocationIcon className="size-[16px]" />
              <span>{locationAddress}</span>
              <span className="ml-auto">Table {tableNumber}</span>
            </Text>
            <Text variant="bodyBold">
              Table seating capacity: {capacity} people
            </Text>
            <Text variant="bodyBold">
              {availableSlots.length} slots available for {format(date, "PP")}:
            </Text>
          </div>
          <div className="grid gap-[0.5rem] @min-[400px]:grid-cols-2">
            {availableSlots.slice(0, 5).map((slot, i) => (
              <MakeReservationDialog
                table={table}
                key={i}
                bookingForm={bookingForm}
              >
                <TimeSlot
                  key={slot.start + slot.end}
                  icon={<ClockIcon className="size-[1rem] stroke-primary" />}
                >
                  {timeStringFrom24hTo12h(slot.start)} -{" "}
                  {timeStringFrom24hTo12h(slot.end)}
                </TimeSlot>
              </MakeReservationDialog>
            ))}
            {availableSlots.length > 0 && (
              <AvailableTimeSlotsDialog
                table={table}
                date={date}
                bookingForm={bookingForm}
              >
                <TimeSlot
                  icon={<PlusIcon className="size-[1rem]" />}
                  className="place-self-start"
                >
                  Show all
                </TimeSlot>
              </AvailableTimeSlotsDialog>
            )}
          </div>
        </div>
      </article>
    </li>
  );
};

// const availableSlotsMock = [
//   "10:30 a.m. - 12:00 p.m",
//   "12:15 p.m. - 1:45 p.m",
//   "2:00 p.m. - 3:30 p.m",
//   "3:45 p.m. - 5:15 p.m",
//   "5:30 p.m. - 7:00 p.m",
//   "8:30 p.m. - 9:30 p.m",
// ];
