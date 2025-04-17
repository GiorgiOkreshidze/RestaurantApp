import {
  formatDateToUI,
  formatTimeToUI,
} from "@/utils/dateTime";
import {
  Button,
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  Text,
} from "../ui";
import type { Dispatch, ReactElement } from "react";
import { toast } from "react-toastify";

export const ReservationConfirmDialog = ({
  open,
  onOpenChange,
  className,
  children,
  guests,
  date,
  time,
  tableNumber,
  locationAddress,
  setIsReservationDialogOpen,
  onCancelReservation,
}: {
  open: boolean;
  onOpenChange: Dispatch<React.SetStateAction<boolean>>;
  className?: string;
  children?: ReactElement;
  guests: number;
  date: string;
  time: string;
  tableNumber: string;
  locationAddress: string;
  setIsReservationDialogOpen: Dispatch<React.SetStateAction<boolean>>;
  onCancelReservation: () => Promise<void>;
}) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogTrigger className={className} asChild>
        {children}
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="!fontset-h2">
            Reservation Confirmed!
          </DialogTitle>
          <DialogDescription className="!fontset-body text-foreground flex flex-col gap-[0.75rem]">
            <Text tag="span" className="!fontset-body text-foreground">
              Your table reservation at <b>Green & Tasty</b> for{" "}
              <b>{guests} people</b> on <b>{formatDateToUI(date)}</b>,
              from{" "}
              <b>
                {time && formatTimeToUI(time.split(" - ")[0])}
              </b>{" "}
              to{" "}
              <b>
                {time && formatTimeToUI(time.split(" - ")[1])}
              </b>{" "}
              at <b>Table {tableNumber}</b> has been successfully made.
            </Text>
            <Text tag="span" className="!fontset-body text-foreground">
              We look forward to welcoming you at <b>{locationAddress}</b>.
            </Text>
            <Text tag="span" className="!fontset-body text-foreground">
              If you need to modify or cancel your reservation, you can do so up
              to 30 min. before the reservation time.
            </Text>
          </DialogDescription>
        </DialogHeader>
        <footer className="grid grid-cols-2 gap-[1rem]">
          <Button
            variant="secondary"
            size="l"
            onClick={async () => {
              await onCancelReservation();
              toast.success("Reservation canceled successfully");
              onOpenChange(false);
            }}
          >
            Cancel Reservation
          </Button>
          <Button
            variant="primary"
            size="l"
            onClick={() => {
              setIsReservationDialogOpen(true);
              onOpenChange(false);
            }}
          >
            Edit Reservation
          </Button>
        </footer>
      </DialogContent>
    </Dialog>
  );
};
