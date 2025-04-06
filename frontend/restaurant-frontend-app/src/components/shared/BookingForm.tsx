import { useBookingForm } from "@/hooks/useBookingForm";
import { DatePicker, GuestsNumber, LocationPicker, TimeSlotPicker } from ".";
import { useSelector } from "react-redux";
import {
  selectSelectOptions,
  selectSelectOptionsLoading,
} from "@/app/slices/locationsSlice";
import { Button } from "../ui/Button";
import { cn } from "../../lib/utils";
import { selectTablesLoading } from "@/app/slices/tablesSlice";
import { Spinner } from "../ui";

export const BookingForm = (props: Props) => {
  const form = useBookingForm();
  const selectOptions = useSelector(selectSelectOptions);
  const selectOptionsLoading = useSelector(selectSelectOptionsLoading);
  const tablesLoading = useSelector(selectTablesLoading);

  return (
    <form
      onSubmit={form.onSubmit}
      className={cn(
        "grid gap-[1rem] md:grid-cols-2 xl:grid-cols-[2fr_repeat(4,minmax(max-content,1fr))] items-start",
        props.className,
      )}
    >
      <LocationPicker
        locationList={selectOptions}
        locationId={form.locationId}
        setLocation={form.setLocation}
        loading={selectOptionsLoading}
      />

      <DatePicker value={form.date} setValue={form.setDate} />

      <TimeSlotPicker
        items={form.timeSlots}
        value={form.time}
        setValue={form.setTime}
        selectedDate={form.date}
        disablePastTimes
      />

      <GuestsNumber
        guests={form.guests}
        increase={form.increaseGuests}
        decrease={form.decreaseGuests}
      />

      <Button type="submit" className="md:max-xl:col-span-2 text-nowrap">
        {tablesLoading ? (
          <Spinner color="var(--color-white)" className="size-[1.5rem]" />
        ) : (
          "Find a Table"
        )}
      </Button>
    </form>
  );
};

interface Props {
  className?: string;
}
