import { PageBody, PageHeading, ReservationCard } from "@/components/shared";
import calendarCrossed from "@/assets/images/calendar-crossed.png";
import { Spinner, Text } from "@/components/ui";
import { Link } from "react-router";
import {
  selectReservations,
  selectReservationsLoading,
} from "@/app/slices/reservationsSlice";
import { useSelector } from "react-redux";
import { cn } from "@/lib/utils";
import { buttonVariants } from "@/components/ui/Button";
import { useEffect } from "react";
import { useAppDispatch } from "@/app/hooks";
import { getReservations } from "@/app/thunks/reservationsThunks";

export const Reservations = () => {
  const dispatch = useAppDispatch();
  const reservations = useSelector(selectReservations);
  const reservationsLoading = useSelector(selectReservationsLoading);

  useEffect(() => {
    dispatch(getReservations({}));
  }, []);

  return (
    <>
      <PageHeading />
      <PageBody variant="smallerPadding" className="grow content-center">
        {reservationsLoading ? (
          <Spinner />
        ) : reservations?.length > 0 ? (
          <div className="grow-1 content-start grid gap-[2rem] lg:grid-cols-[repeat(auto-fit,minmax(350px,1fr))]">
            {reservations.map((reservation) => (
              <ReservationCard key={reservation.id} reservation={reservation} />
            ))}
          </div>
        ) : (
          <div className="grow-1 flex items-center justify-center">
            <div className="flex flex-col justify-center items-center">
              <img
                src={calendarCrossed}
                alt="No Reservations"
                className="w-[135px] h-[135px]"
              />
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
      </PageBody>
    </>
  );
};
