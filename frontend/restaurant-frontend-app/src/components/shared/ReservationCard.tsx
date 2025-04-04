import { Card } from "./Card";
import { Badge, Button, Text } from "../ui";
import { CalendarIcon, ClockIcon, LocationIcon, PeopleIcon } from "../icons";

import { ClientReservationDialog } from "./ClientReservationDialog";
import { Reservation } from "@/types/reservation.types";
import { useAppDispatch } from "@/app/hooks";
import {
  deleteClientReservation,
  getReservations,
} from "@/app/thunks/reservationsThunks";
import { isPast, parse } from "date-fns";
import {
  dateObjToDateStringUI,
  dateStringServerToDateObject,
  timeString24hToTimeString12h,
} from "@/utils/dateTime";
import { useSelector } from "react-redux";
import { selectUser } from "@/app/slices/userSlice";
import { USER_ROLE } from "@/utils/constants";

export const ReservationCard = (props: { reservation: Reservation }) => {
  const dispatch = useAppDispatch();
  const user = useSelector(selectUser);
  const { reservation } = props;

  return (
    <Card {...props} className="flex flex-col gap-[3rem]">
      <div className="flex items-start justify-between gap-[0.5rem]">
        <div className="flex flex-col gap-[0.5rem]">
          <div className="flex items-center gap-[0.5rem]">
            <LocationIcon className="size-[16px] text-primary" />
            <Text tag="span" variant="bodyBold">
              {reservation.locationAddress}, Table {reservation.tableNumber}
            </Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <CalendarIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">
              {dateObjToDateStringUI(reservation.date)}
            </Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <ClockIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">
              {timeString24hToTimeString12h(
                reservation.timeSlot.split(" - ")[0],
              )}
            </Text>
            <Text variant="bodyBold"> - </Text>
            <Text variant="bodyBold">
              {timeString24hToTimeString12h(
                reservation.timeSlot.split(" - ")[1],
              )}
            </Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <PeopleIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">{reservation.guestsNumber} Guests</Text>
          </div>
        </div>
        <Badge status={reservation.status} className="text-nowrap">
          {reservation.status}
        </Badge>
      </div>
      <footer className="flex items-center gap-[1rem] justify-end">
        <Button
          variant="tertiary"
          size="sm"
          className="relative before:absolute before:content[''] before:bottom-0 before:left-0 before:w-full before:h-[1px] before:bg-black disabled:before:bg-disabled"
          disabled={
            reservation.status === "Cancelled" ||
            isPast(
              parse(reservation.editableTill, "yyyy-MM-dd HH:mm", new Date()),
            )
          }
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
        <Button
          variant="secondary"
          size="l"
          disabled={
            reservation.status === "Cancelled" ||
            isPast(
              parse(reservation.editableTill, "yyyy-MM-dd HH:mm", new Date()),
            )
          }
        >
          {user?.role === USER_ROLE.CUSTOMER ? "Edit" : "Postpone"}
        </Button>
      </footer>
    </Card>
  );
};
