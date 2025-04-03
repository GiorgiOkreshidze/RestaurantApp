import { PropsWithChildren, useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  Label,
  RadioGroup,
  RadioGroupItem,
} from "../ui";
import { GuestsNumber } from "./GuestsNumber";

interface Props extends PropsWithChildren {
  className?: string;
}

interface ReservationData {
  locationId: string;
  tableNumber: number;
  date: Date;
  guestsNumber: number;
  timeFrom: string;
  timeTo: string;
}

export const WaiterReservationDialog: React.FC<Props> = ({
  className,
  children,
}) => {
  const [open, setOpen] = useState(false);
  const [guests, setGuests] = useState(2);

  const onSubmit = (data) => {
    console.log("Submitted Data:", data);
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger className={className} asChild>
        {children}
      </DialogTrigger>

      <DialogContent>
        <DialogHeader>
          <DialogTitle className="!fontset-h2">New Reservation</DialogTitle>
        </DialogHeader>

        {/* <form onSubmit={onSubmit}> */}
        {/* <RadioGroup defaultValue="visitor">
          <div className="flex items-center">
            <RadioGroupItem value="visitor" id="visitor" />
            <Label htmlFor="visitor">Visitor</Label>
          </div>
          <div className="flex items-center">
            <RadioGroupItem value="customer" id="customer" />
            <Label htmlFor="customer">Customer</Label>
          </div>
        </RadioGroup> */}

        <div>
          <GuestsNumber
            guests={guests}
            increase={() => setGuests((prev) => Math.min(prev + 1, 10))}
            decrease={() => setGuests((prev) => Math.max(prev - 1, 2))}
          />
        </div>
        {/* </form> */}
      </DialogContent>
    </Dialog>
  );
};
