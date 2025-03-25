import { useBookingForm } from "@/hooks/useBookingForm";
import { ChevronDownIcon, ClockIcon } from "../icons";
import { Button, Text } from "../ui";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui";
import { Tooltip } from "./Tooltip";

export const TimeSlotsDialog = ({ className }: { className?: string }) => {
  const { locationId, date } = useBookingForm();
  let message = "Please select ";
  if (!locationId) {
    message = message.concat('"Location"');
  }
  if (!date && !locationId) {
    message = message.concat(' and "Date"');
  } else if (!date) {
    message = message.concat('"Date"');
  }
  message = message.concat(" first");

  return (
    <Dialog>
      <Tooltip
        message={message}
        delayDuration={0}
        disabled={Boolean(locationId && date)}
        asChild
      >
        <DialogTrigger
          className={className}
          asChild
          disabled={!locationId || !date}
        >
          <Button variant="trigger" size="trigger">
            <ClockIcon className="size-[24px]" />
            <Text variant="buttonSecondary">Time</Text>
            <ChevronDownIcon className="text-foreground ml-auto" />
          </Button>
        </DialogTrigger>
      </Tooltip>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Available slots</DialogTitle>
          <DialogDescription>
            There are 7 slots available at 48 Rustaveli Avenue, Table 1, for
            October 14, 2024
          </DialogDescription>
        </DialogHeader>
        <div className="grid grid-cols-2 gap-[0.5rem]">
          {timeSlots.map((value, i) => (
            <Button
              variant="secondary"
              size="sm"
              key={i}
              className="font-normal text-foreground flex gap-[0.5rem] justify-start fontset-bodyBold"
            >
              <ClockIcon />
              {value}
            </Button>
          ))}
        </div>
      </DialogContent>
    </Dialog>
  );
};

const timeSlots = [
  "10:30 a.m. - 12:00 p.m",
  "12:15 p.m. - 1:45 p.m",
  "2:00 p.m. - 3:30 p.m",
  "3:45 p.m. - 5:15 p.m",
  "5:30 p.m. - 7:00 p.m",
  "7:15 p.m. - 8:45 p.m",
  "9:00 p.m. - 10:30 p.m",
];
