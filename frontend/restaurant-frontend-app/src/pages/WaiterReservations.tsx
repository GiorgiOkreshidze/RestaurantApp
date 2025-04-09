import {
  WaiterReservationDialog,
  PageBody,
  PageHeading,
  ReservationCard,
  WaiterReservationsForm,
} from "@/components/shared";
import { useSelector } from "react-redux";
import {
  selectReservations,
  selectReservationsLoading,
} from "@/app/slices/reservationsSlice";
import { useEffect } from "react";
import { useAppDispatch } from "@/app/hooks";
import { getReservations } from "@/app/thunks/reservationsThunks";
import { Button, Spinner, Text } from "@/components/ui";
import { useWaiterReservations } from "@/hooks/useWaiterReservations";
import {
  dateObjToDateStringUI,
  timeString24hToTimeString12h,
} from "@/utils/dateTime";
import { PlusIcon } from "lucide-react";
import { cn } from "@/lib/utils";
import { buttonVariants } from "@/components/ui/Button";
import { Link } from "react-router";
import { getAllUsers } from "@/app/thunks/userThunks";

export const WaiterReservation = () => {
  const store = useWaiterReservations();
  const dispatch = useAppDispatch();
  const reservations = useSelector(selectReservations);
  const reservationsLoading = useSelector(selectReservationsLoading);

  useEffect(() => {
    dispatch(getReservations({}));
    dispatch(getAllUsers());
  }, [dispatch]);

  return (
    <>
      <PageHeading />
      <PageBody variant="smallerPadding">
        <WaiterReservationsForm />
        <div className="flex flex-col gap-[1rem] my-[2rem] justify-between items-center md:flex-row">
          <Text variant="h3">
            You have {reservations.length} reservations{" "}
            {store.date ? dateObjToDateStringUI(store.date) : ""}{" "}
            {store?.time
              ? `, ${store.time
                  .split(" - ")
                  .map((time) => timeString24hToTimeString12h(time))
                  .join(" - ")}`
              : null}
          </Text>
          <WaiterReservationDialog
            date={store.date}
            time={store.time}
            table={store.table}
          >
            <Button className="gap-[0.5rem]">
              <PlusIcon />
              <span>Create New Reservation</span>
            </Button>
          </WaiterReservationDialog>
        </div>
        <div className="grow content-center">
          {reservationsLoading ? (
            <Spinner />
          ) : reservations?.length > 0 ? (
            <div className="grid gap-[2rem] lg:grid-cols-[repeat(auto-fill,minmax(350px,1fr))]">
              {reservations.map((reservation) => (
                <ReservationCard
                  key={reservation.id}
                  reservation={reservation}
                />
              ))}
            </div>
          ) : (
            <div className="grow-1 flex items-center justify-center">
              <div className="flex flex-col justify-center items-center">
                {/* <img
                  src={calendarCrossed}
                  alt="No Reservations"
                  className="w-[135px] h-[135px]"
                /> */}
                <div className="flex flex-col items-center mt-[2.5rem]">
                  <Text variant="h3">No Reservations </Text>
                  <Text variant="body">
                    Looks like you haven't made any reservations yet.
                  </Text>
                </div>
                <Link
                  to="/booking"
                  className={cn(
                    buttonVariants({ variant: "primary", size: "xl" }),
                    "mt-[1.5rem] w-full",
                  )}
                >
                  Book a Table
                </Link>
              </div>
            </div>
          )}
        </div>
      </PageBody>
    </>
  );
};
