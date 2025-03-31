import {
  Container,
  DatePicker,
  PageHeading,
  ReservationCard,
  Select,
  TimeSlotPicker,
  WaiterReservationDialog,
} from "@/components/shared";
import { Button, Dialog, Text } from "@/components/ui";
import { useSelector } from "react-redux";
import { selectReservations } from "@/app/slices/reservationsSlice";
import { useEffect } from "react";
import { useAppDispatch } from "@/app/hooks";
import { getReservations } from "@/app/thunks/reservationsThunks";
import { format } from "date-fns";
import { PlusIcon, SearchIcon } from "lucide-react";
import { useForm, Controller } from "react-hook-form";
import { TIME_SLOTS } from "@/utils/constants";

interface WaiterReservationData {
  date: Date;
  time: string;
  tableNumber: string;
}

export const WaiterReservation = () => {
  const dispatch = useAppDispatch();
  const reservations = useSelector(selectReservations);

  const { control, handleSubmit, setValue, watch } = useForm({
    defaultValues: {
      date: new Date(),
      time: "",
      tableNumber: "",
    },
  });

  const date = watch("date");
  const time = watch("time");

  useEffect(() => {
    dispatch(getReservations());
  }, [dispatch]);

  const onSubmit = (data: WaiterReservationData) => {
    console.log("Submitted Data:", data);
  };

  return (
    <>
      <PageHeading />
      <Container className="flex flex-col grow-1">
        <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6">
          <div className="flex items-center justify-center gap-4 mt-10 mb-8">
            <div className="w-[200px]">
              <Controller
                name="date"
                control={control}
                render={({ field }) => (
                  <DatePicker value={field.value} setValue={field.onChange} />
                )}
              />
            </div>

            <div className="w-[185px]">
              <Controller
                name="time"
                control={control}
                render={({ field }) => (
                  <TimeSlotPicker
                    items={TIME_SLOTS}
                    value={field.value}
                    setValue={field.onChange}
                    selectedDate={date}
                  />
                )}
              />
            </div>

            <div className="w-[200px]">
              <Controller
                name="tableNumber"
                control={control}
                render={({ field }) => (
                  <Select
                    items={["1", "2", "3", "4", "5", "6"].map((i) => ({
                      id: i,
                      label: "Table " + i,
                    }))}
                    placeholder="Any table"
                    value={field.value}
                    setValue={field.onChange}
                    className="w-full"
                  />
                )}
              />
            </div>

            <Button type="submit" variant="secondary" className="!min-w-[44px]">
              <SearchIcon />
            </Button>
          </div>

          <div className="flex items-center justify-between">
            <Text variant="h3">
              You have {reservations.length} reservations for{" "}
              {format(date, "MMM d, yyyy")} at {time}
            </Text>

            <WaiterReservationDialog>
              <Button icon={<PlusIcon className="mr-3" />}>
                Create New Reservation
              </Button>
            </WaiterReservationDialog>
          </div>
        </form>

        <div className="grow-1 content-start grid gap-[2rem] lg:grid-cols-[repeat(auto-fit,minmax(350px,1fr))]">
          {reservations.map((reservation) => (
            <ReservationCard key={reservation.id} reservation={reservation} />
          ))}
        </div>
      </Container>

      <Dialog></Dialog>
    </>
  );
};
