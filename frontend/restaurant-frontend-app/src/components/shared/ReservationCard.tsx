import { Badge, Button, Text } from "../ui";
import { CalendarIcon, ClockIcon, LocationIcon, PeopleIcon } from "../icons";
import { Reservation } from "@/types/reservation.types";
import { useAppDispatch } from "@/app/hooks";
import {
  deleteClientReservation,
  getReservations,
} from "@/app/thunks/reservationsThunks";
import {
  formatDateToUI,
  parseDateFromServer,
  formatTimeToUI,
} from "@/utils/dateTime";
import { useSelector } from "react-redux";
import { selectUser } from "@/app/slices/userSlice";
import { ReservationFeedbackDialog } from ".";
import { ClientReservationDialog } from "./ClientReservationDialog";
import { Preorder } from "@/types/preorder.types";
import { setActivePreorder } from "@/app/slices/preordersSlice";
import { useNavigate } from "react-router";

export const ReservationCard = (props: { reservation: Reservation }) => {
  const { reservation } = props;
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const user = useSelector(selectUser);

  const handlePreorderClick = (preorderId: Preorder["id"]) => {
    dispatch(setActivePreorder(preorderId));
    navigate("/menu");
  };

  return (
    <article className="styleSet-card flex flex-col gap-[3rem] @container">
      <div className="flex flex-col gap-[1rem] items-start justify-between @xs:flex-row-reverse">
        <Badge status={reservation.status} className="text-nowrap">
          {reservation.status}
        </Badge>
        <div className="flex flex-col gap-[0.5rem]">
          <div className="flex items-center gap-[0.5rem]">
            <LocationIcon className="size-[16px] text-primary" />
            <Text tag="span" variant="bodyBold">
              {reservation.locationAddress}, Table {reservation.tableNumber}
            </Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <CalendarIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">{formatDateToUI(reservation.date)}</Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <ClockIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">
              {formatTimeToUI(reservation.timeSlot.split(" - ")[0])}
            </Text>
            <Text variant="bodyBold"> - </Text>
            <Text variant="bodyBold">
              {formatTimeToUI(reservation.timeSlot.split(" - ")[1])}
            </Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <PeopleIcon className="size-[16px] stroke-primary" />
            <Text variant="bodyBold">{reservation.guestsNumber} Guests</Text>
          </div>
        </div>
      </div>
      <footer className="flex flex-col gap-[1rem] @xs:flex-row">
        <Button
          variant="underlined"
          size="sm"
          disabled={reservation.status === "Cancelled"}
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
        {reservation.status === "In Progress" && user?.role === "Customer" ? (
          <ReservationFeedbackDialog reservationId={reservation.id}>
            <Button variant="secondary" size="l">
              Leave Feadback
            </Button>
          </ReservationFeedbackDialog>
        ) : (
          <ClientReservationDialog
            key={reservation.id}
            reservationId={reservation.id}
            locationId={reservation.locationId}
            locationAddress={reservation.locationAddress}
            tableId={reservation.tableId}
            tableNumber={reservation.tableNumber}
            date={parseDateFromServer(reservation.date)}
            initTime={reservation.timeSlot}
            initGuests={Number.parseInt(reservation.guestsNumber)}
            maxGuests={Number.parseInt(reservation.tableCapacity)}
          >
            <Button
              variant="secondary"
              size="l"
              disabled={reservation.status === "Cancelled"}
              className="min-w-[100px]"
            >
              {user?.role === "Customer" ? "Edit" : "Postpone"}
            </Button>
          </ClientReservationDialog>
        )}

        {reservation.status === "Reserved" && user?.role === "Customer" && (
          <Button
            variant="primary"
            size="l"
            onClick={() => handlePreorderClick(reservation.preOrder)}
          >
            Pre-order
          </Button>
        )}
      </footer>
    </article>
  );
};
