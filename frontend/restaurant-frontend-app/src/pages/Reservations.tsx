import { Container, PageTitle, ReservationCard } from "@/components/shared";
import { Reservation } from "@/types";
import calendarCrossed from "@/assets/images/calendar-crossed.png";
import { Button, Text } from "@/components/ui";
import { Link } from "react-router";

export const Reservations = () => {
  return (
    <>
      <PageTitle />
      <Container className="flex flex-col grow-1">
        {reservations.length > 0 ? (
          <div className="grow-1 content-start grid gap-[2rem] lg:grid-cols-[repeat(auto-fit,minmax(350px,1fr))]">
            {reservations.map((reservation) => (
              <ReservationCard {...reservation} />
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
    location: "48 Rustaveli Avenue",
    status: "Reserved",
    date: "Oct 14, 2024",
    time: "12:15 p.m. - 1:45 p.m.",
    guests: "10",
  },

  {
    location: "14 Baratashvili Street",
    status: "Reserved",
    date: "Oct 16, 2024",
    time: "10:30 a.m. - 12:00 p.m.",
    guests: "10",
  },

  {
    location: "14 Baratashvili Street",
    status: "In Progress",
    date: "Sep 14, 2024",
    time: "10:30 a.m. - 11:30 a.m.",
    guests: "5",
  },

  {
    location: "14 Baratashvili Street",
    status: "Finished",
    date: "Jun 6, 2024",
    time: "10:30 a.m. - 11:30 a.m.",
    guests: "4",
  },

  {
    location: "14 Baratashvili Street",
    status: "Canceled",
    date: "Mar 28, 2024",
    time: "10:30 a.m. - 11:30 a.m.",
    guests: "2",
  },
];
