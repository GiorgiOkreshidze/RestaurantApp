import { Button } from "../ui";
import { DatePicker } from "./DatePicker";
import { ComponentProps } from "react";
import { cn } from "@/lib/utils";
import { useBookingForm } from "@/hooks/useBookingForm";
import { GuestsNumber, Select } from "@/components/shared";
import { useSelector } from "react-redux";
import { selectLocations } from "@/app/slices/locationsSlice";
import { TimePicker } from "./TimePicker";
import { Form, FormField, FormItem, FormMessage } from "@/components/ui/";
import { selectFilters } from "@/app/slices/bookingSlice";

export const BookingForm = ({
  className,
  ...props
}: ComponentProps<"form">) => {
  const locations = useSelector(selectLocations);
  const {
    locationId,
    setLocationId,
    date,
    setDate,
    onSubmit,
    form,
    setTime,
    guests,
    increaseGuestsNumber,
    decreaseGuestsNumber,
  } = useBookingForm();
  const filters = useSelector(selectFilters);
  // console.log(filters);
  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className={cn(
          "grid gap-[1rem] md:grid-cols-2 xl:grid-cols-[2fr_repeat(4,minmax(max-content,1fr))] items-start",
          className,
        )}
        {...props}
      >
        <FormField
          control={form.control}
          name="locationId"
          render={({ field: { onChange } }) => {
            return (
              <FormItem>
                <Select
                  items={locations.map((location) => ({
                    id: location.id,
                    label: location.address,
                  }))}
                  placeholder="Location"
                  value={locationId}
                  setValue={(id) => {
                    setLocationId(id);
                    onChange(id);
                  }}
                  className="w-full"
                />
                <FormMessage className="bg-red-100 rounded-[4px] p-[0.5rem]" />
              </FormItem>
            );
          }}
        />
        <DatePicker value={date} setValue={setDate} className="w-full" />
        <TimePicker date={date} setDate={setTime} />
        {/* <Select
          items={locations.map((location) => ({
            id: location.id,
            label: location.address,
          }))}
          placeholder="Location"
          value={locationId}
          setValue={(id) => {
            setLocationId(id);
            onChange(id);
          }}
          className="w-full"
        /> */}
        <GuestsNumber
          guests={guests}
          increase={increaseGuestsNumber}
          decrease={decreaseGuestsNumber}
        />
        <Button type="submit" className="md:max-xl:col-span-2">
          Find&nbsp;a&nbsp;Table
        </Button>
      </form>
    </Form>
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
