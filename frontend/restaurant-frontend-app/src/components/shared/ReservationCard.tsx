import { ComponentProps } from "react";
import { Card } from "./Card";
import { Badge, Button, Text } from "../ui";
import { CalendarIcon, ClockIcon, LocationIcon, PeopleIcon } from "../icons";

import { ReservationDialog } from "./ReservationDialog";
import { Reservation } from "@/types/reservation.types";
import { dateStringServerToDateObject } from "@/utils/dateTime";
import { useAppDispatch } from "@/app/hooks";
import {
  deleteClientReservation,
  getReservations,
} from "@/app/thunks/reservationsThunks";

export const ReservationCard = ({
  reservation,
  ...props
}: ComponentProps<"div"> & { reservation: Reservation }) => {
  const dispatch = useAppDispatch();
  const {
    id,
    locationAddress,
    status,
    date,
    timeSlot,
    guestsNumber,
    tableNumber,
  } = reservation;
  return (
    <Card {...props} className="flex flex-col gap-[3rem]">
      <div className="flex items-start justify-between gap-[0.5rem]">
        <div className="flex flex-col gap-[0.5rem]">
          <div className="flex items-center gap-[0.5rem]">
            <LocationIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">{locationAddress}</Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <CalendarIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">{date}</Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <ClockIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">{timeSlot}</Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <PeopleIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">{guestsNumber} Guests</Text>
          </div>
        </div>
        <Badge status={status} className="text-nowrap">
          {status}
        </Badge>
      </div>
      <footer className="flex items-center gap-[1rem] justify-end">
        <Button
          variant="tertiary"
          size="sm"
          className="relative before:absolute before:content[''] before:bottom-0 before:left-0 before:w-full before:h-[1px] before:bg-black disabled:before:bg-disabled"
          disabled={status === "Cancelled"}
          onClick={async () => {
            try {
              await dispatch(deleteClientReservation(reservation.id)).unwrap();
              await dispatch(getReservations({})).unwrap();
              console.log("Reservation deleted");
            } catch (error) {
              console.log("Reservation deleting failed:", error);
            }
          }}
        >
          Cancel
        </Button>
        <ReservationDialog
          key={id}
          locationAddress={locationAddress}
          date={dateStringServerToDateObject(date)}
          initTime={timeSlot.replace(/\s/g, "")}
          tableNumber={tableNumber}
          initGuests={Number.parseInt(guestsNumber)}
          maxGuests={Number.parseInt(guestsNumber)}
          locationId={""}
          tableId={""}
        >
          <Button
            variant="secondary"
            size="l"
            disabled={status === "Cancelled"}
          >
            Edit
          </Button>
        </ReservationDialog>
      </footer>
    </Card>
  );
};
