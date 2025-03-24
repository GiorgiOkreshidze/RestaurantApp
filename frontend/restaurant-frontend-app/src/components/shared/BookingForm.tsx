import { LocationIcon } from "../icons";
import { Button, Form } from "../ui";
import { DatePicker } from "./DatePicker";
import { ComponentProps } from "react";
import { cn } from "@/lib/utils";
import { useSelector } from "react-redux";
import { selectLocations } from "@/app/slices/locationsSlice";
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
  const { form, onSubmit } = useBookingForm();
  const locations = useSelector(selectLocations);

  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className={cn(
          "flex flex-col flex-wrap gap-[1rem] *:grow-1 md:flex-row",
          // "grid gap-[1rem] md:grid-cols-[repeat(auto-fit,minmax(300px,1fr))] xl:grid-cols-[1.5fr_1fr_1fr_1fr_1fr]",
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
          className="md:basis-[350px]"
        />
        <DatePicker />
        <AvailableSlotsDialog className="md:basis-[200px]" />
        <GuestsNumber className="md:grow-0" />
        <Button className="">Find&nbsp;a&nbsp;Table</Button>
      </form>
    </Form>
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

// const locations = [
//   { id: "1", label: "Hello 1" },
//   { id: "2", label: "Hello 2" },
//   { id: "3", label: "Hello 3" },
// ];
