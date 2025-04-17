import { InfoBar } from "./InfoBar";
import {
  formatDateToUI,
  parseDateFromServer,
  formatTimeToUI,
} from "@/utils/dateTime";
import { Button } from "../ui";
import { useAppDispatch } from "@/app/hooks";

import { X } from "lucide-react";
import {
  selectActivePreorderId,
  setActivePreorder,
} from "@/app/slices/preordersSlice";
import { useSelector } from "react-redux";
import { selectReservations } from "@/app/slices/reservationsSlice";

export const PreorderInfoBar = () => {
  const dispatch = useAppDispatch();
  const activePreorderId = useSelector(selectActivePreorderId);
  const allReservations = useSelector(selectReservations);
  const activeReservation = allReservations.find(
    (reservation) => reservation.preOrder === activePreorderId,
  );
  
  if (!activeReservation) {
    return null;
  }

  return (
    <InfoBar>
      <span>
        You are making a pre-order for the reservation{" "}
        <b>
          {activeReservation.locationAddress}, Table{" "}
          {activeReservation.tableNumber},{" "}
          {formatDateToUI(
            parseDateFromServer(activeReservation.date),
          )}
          , {formatTimeToUI(activeReservation.timeFrom)} -{" "}
          {formatTimeToUI(
            activeReservation.timeSlot.split(" - ")[1],
          )}
        </b>
      </span>
      <Button
        variant="secondary"
        size="sm"
        onClick={() => dispatch(setActivePreorder(null))}
        className="bg-transparent hover:bg-neutral-0"
      >
        <X className="size-[1rem]" />
      </Button>
    </InfoBar>
  );
};
