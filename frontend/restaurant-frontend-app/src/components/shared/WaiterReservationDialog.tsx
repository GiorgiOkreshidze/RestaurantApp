import { PropsWithChildren, useState } from "react";
import {
  Button,
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  Spinner,
  Text,
} from "../ui";
import { useWaiterReservationDialog } from "@/hooks/useWaiterReservationDialog";
import {
  CustomerPicker,
  DatePicker,
  GuestsNumber,
  TablePicker,
  TimeSlotPicker,
  UserPicker,
} from ".";
import { UserType } from "@/types/user.types";
import { LocationIcon } from "../icons";
import { useSelector } from "react-redux";
import { selectBooking } from "@/app/slices/bookingSlice";
import { selectLocationTables } from "@/app/slices/locationsSlice";

export const WaiterReservationDialog = (props: Props) => {
  const booking = useSelector(selectBooking);
  const [isCurrentDialogOpen, setIsCurrentDialogOpen] = useState(false);
  const onSuccessCallback = () => {
    setIsCurrentDialogOpen(!isCurrentDialogOpen);
  };
  const state = useWaiterReservationDialog({
    initTable: props.table,
    initDate: props.date,
    onSuccessCallback,
  });
  const locationTables = useSelector(selectLocationTables);

  return (
    <Dialog open={isCurrentDialogOpen} onOpenChange={setIsCurrentDialogOpen}>
      <DialogTrigger className={props.className} asChild>
        {props.children}
      </DialogTrigger>
      <DialogContent className="bg-neutral-100">
        <DialogHeader>
          <DialogTitle className="!fontset-h2">New Reservation</DialogTitle>
        </DialogHeader>
        <form className="flex flex-col gap-[2rem]" onSubmit={state.onSubmit}>
          <div className="border-neutral-200 bg-card py-[1rem] px-[1.5rem] flex items-center gap-[0.5rem] border rounded">
            <LocationIcon className="text-primary" />
            <Text variant="bodyBold">
              {state.selectOptionsLoading
                ? "Loading..."
                : state.selectOptions.find(
                    (loc) => loc.id === state.waiter?.locationId,
                  )?.address}
            </Text>
          </div>
          <UserPicker
            userType={state.userType}
            setUserType={state.setUserType}
          />
          {state.userType === UserType.Customer && (
            <CustomerPicker
              customerId={state.customerId}
              setCustomerId={state.setCustomerId}
              customerList={state.allCustomers}
            />
          )}

          <GuestsNumber
            guests={state.guests}
            increase={state.increaseGuests}
            decrease={state.decreaseGuests}
          />

          <div>
            <Text variant="h3">Date & Time</Text>
            <Text variant="body">
              Please choose your preferred time from the dropdowns below
            </Text>
            <DatePicker
              value={state.date}
              setValue={state.setDate}
              className="mt-[1rem] w-full"
            />
            <TimeSlotPicker
              items={booking.timeSlots}
              value={state.time}
              setValue={state.setTime}
              selectedDate={state.date}
              disablePastTimes
              className="mt-[1rem]"
            />
          </div>

          <TablePicker
            table={state.table}
            tableList={locationTables.filter(
              (table) => table.locationId === state.waiter?.locationId,
            )}
            setTable={state.setTable}
          />
          <DialogFooter>
            <Button type="submit" className="w-full">
              {state.reservationCreatingLoading ? (
                <Spinner />
              ) : (
                "Make a Reservation"
              )}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};

interface Props extends PropsWithChildren {
  className?: string;
  date: string;
  time: string;
  table: string;
}
