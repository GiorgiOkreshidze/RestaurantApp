import { LocationIcon } from "../icons";
import { Button } from "../ui";
import { DatePicker } from "./DatePicker";
import { ComponentProps } from "react";
import { cn } from "@/lib/utils";
import { useBookingForm } from "@/hooks/useBookingForm";
import {
  AvailableSlotsDialog,
  GuestsNumber,
  Select,
} from "@/components/shared";

export const BookingForm = ({
  className,
  ...props
}: ComponentProps<"form">) => {
  // const locations = useSelector(selectLocations);
  const {
    locationId,
    setLocationId,
    date,
    setDate,
    guestsNumber,
    setGuestsNumber,
  } = useBookingForm();

  return (
    <form
      className={cn(
        "grid gap-[1rem] md:grid-cols-2 xl:grid-cols-[repeat(5,1fr)] 2xl:grid-cols-[2fr_1fr_1fr_1fr_1fr]",
        className,
      )}
      {...props}
    >
      <Select
        items={locations.map((location) => ({
          id: location.id,
          label: location.address,
        }))}
        placeholder="Location"
        Icon={LocationIcon}
        value={locationId}
        setValue={setLocationId}
      />
      <DatePicker value={date} setValue={setDate}/>
      <AvailableSlotsDialog />
      <GuestsNumber value={guestsNumber} setValue={setGuestsNumber} />
      <Button className="md:max-xl:col-span-2">Find&nbsp;a&nbsp;Table</Button>
    </form>
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

const locations = [
  { id: "1", address: "Hello 1" },
  { id: "2", address: "Hello 2" },
  { id: "3", address: "Hello 3" },
];

const 