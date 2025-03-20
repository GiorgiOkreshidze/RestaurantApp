import { Container, PageTitle, ReservationCard } from "@/components/shared";
import calendarCrossed from "@/assets/images/calendar-crossed.png";
import { Button, Text } from "@/components/ui";
import { Link } from "react-router";
// import { useEffect } from "react";
// import { getReservations } from "@/app/thunks/reservationsThunks";
// import { useSelector } from "react-redux";
// import { useAppDispatch } from "@/app/hooks";
// import { selectReservations } from "@/app/slices/reservationsSlice";
import { Reservation } from "@/types";

export const Reservations = () => {
  // const dispatch = useAppDispatch();
  // const reservations = useSelector(selectReservations);

  // useEffect(() => {
  //   if (!reservations.length) {
  //     dispatch(getReservations());
  //   }
  // }, [dispatch, reservations.length]);

  return (
    <>
      <PageTitle />
      <Container className="flex flex-col grow-1">
        {reservations?.length > 0 ? (
          <div className="grow-1 content-start grid gap-[2rem] lg:grid-cols-[repeat(auto-fit,minmax(350px,1fr))]">
            {reservations.map((reservation) => (
              <ReservationCard key={reservation.id} {...reservation} />
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
                  Looks like you haven’t made any reservations yet.
                </Text>
              </div>
              <Button
                variant="primary"
                size="xl"
                className="mt-[1.5rem] w-full"
                asChild
              >
                <Link to="/booking">Book a Table</Link>
              </Button>
            </div>
          </div>
        )}
      </Container>
    </>
  );
};

const reservations: Reservation[] = [
  {
    id: "1",
    feedbackId: "1",
    locationAddress: "48 Rustaveli Avenue",
    status: "Reserved",
    date: "Oct 14, 2024",
    timeSlot: "12:15 p.m. - 1:45 p.m.",
    guestsNumber: "10",
    preOrder: "1",
    tableNumber: "1",
    userInfo: "1",
  },
  {
    id: "2",
    feedbackId: "2",
    locationAddress: "14 Baratashvili Street",
    status: "Reserved",
    date: "Oct 16, 2024",
    timeSlot: "10:30 a.m. - 12:00 p.m.",
    guestsNumber: "10",
    preOrder: "2",
    tableNumber: "1",
    userInfo: "1",
  },
  {
    id: "3",
    feedbackId: "3",
    locationAddress: "14 Baratashvili Street",
    status: "In Progress",
    date: "Sep 14, 2024",
    timeSlot: "10:30 a.m. - 11:30 a.m.",
    guestsNumber: "5",
    preOrder: "3",
    tableNumber: "1",
    userInfo: "1",
  },
  {
    id: "4",
    feedbackId: "4",
    locationAddress: "14 Baratashvili Street",
    status: "Finished",
    date: "Jun 6, 2024",
    timeSlot: "10:30 a.m. - 11:30 a.m.",
    guestsNumber: "4",
    preOrder: "4",
    tableNumber: "1",
    userInfo: "1",
  },
  {
    id: "5",
    feedbackId: "5",
    locationAddress: "14 Baratashvili Street",
    status: "Canceled",
    date: "Mar 28, 2024",
    timeSlot: "10:30 a.m. - 11:30 a.m.",
    guestsNumber: "2",
    preOrder: "5",
    tableNumber: "1",
    userInfo: "1",
  },
];
