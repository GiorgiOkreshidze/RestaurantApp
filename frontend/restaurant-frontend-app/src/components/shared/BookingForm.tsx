import { UseBookingForm } from "@/hooks/useBookingForm";
import { cn } from "@/lib/utils";
import { ComponentProps } from "react";
import { Select } from "./Select";
import { DatePicker } from "./DatePicker";
import { TimeSlotPicker } from "./TimeSlotPicker";
import { LocationIcon } from "../icons";
import { GuestsNumber } from "./GuestsNumber";
import { Button } from "../ui";

export const BookingForm = ({
  className,
  bookingForm,
  ...props
}: ComponentProps<"form"> & {
  bookingForm: UseBookingForm;
}) => {
  const {
    onSubmit,
    locationId,
    setLocationId,
    date,
    setDate,
    time,
    setTime,
    guests,
    increaseGuests,
    decreaseGuests,
    selectOptions,
    selectOptionsLoading,
    locationTimeSlots,
    locationTimeSlotsLoading,
  } = bookingForm;
  return (
    <form
      onSubmit={onSubmit}
      className={cn(
        "grid gap-[1rem] md:grid-cols-2 xl:grid-cols-[2fr_repeat(4,minmax(max-content,1fr))] items-start",
        className,
      )}
      {...props}
    >
      <Select
        items={selectOptions.map((option) => ({
          id: option.id,
          label: option.address,
        }))}
        placeholder="Location"
        value={locationId}
        setValue={setLocationId}
        className="w-full"
        Icon={() => <LocationIcon />}
        loading={selectOptionsLoading}
      />

      <DatePicker value={date} setValue={setDate} className="w-full" />

      <TimeSlotPicker
        items={locationTimeSlots}
        value={time}
        setValue={setTime}
        loading={locationTimeSlotsLoading}
        selectedDate={date}
      />

      <GuestsNumber
        guests={guests}
        increase={increaseGuests}
        decrease={decreaseGuests}
      />

      <Button type="submit" className="md:max-xl:col-span-2">
        Find&nbsp;a&nbsp;Table
      </Button>
    </form>
  );
};

// const timeSlotsMock = [
//   "10:30 a.m. - 12:00 p.m",
//   "12:15 p.m. - 1:45 p.m",
//   "2:00 p.m. - 3:30 p.m",
//   "3:45 p.m. - 5:15 p.m",
//   "5:30 p.m. - 7:00 p.m",
//   "7:15 p.m. - 8:45 p.m",
//   "9:00 p.m. - 10:30 p.m",
// ];

// const locationsMock = [
//   { id: "1", address: "Hello 1" },
//   { id: "2", address: "Hello 2" },
//   { id: "3", address: "Hello 3" },
// ];
