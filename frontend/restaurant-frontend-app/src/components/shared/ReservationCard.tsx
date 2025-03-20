import { ComponentProps } from "react";
import { Card } from "./Card";
import { Badge, Button, Text } from "../ui";
import { CalendarIcon, ClockIcon, LocationIcon, PeopleIcon } from "../icons";

import { Reservation } from "@/types";

export const ReservationCard = ({
  id,
  feedbackId,
  locationAddress,
  status,
  date,
  timeSlot,
  guestsNumber,
  preOrder,
  tableNumber,
  userInfo,
  children,
  ...props
}: ComponentProps<"div"> & Reservation) => {
  return (
    <Card {...props} className="flex flex-col gap-[3rem]">
      <div className="flex items-start justify-between gap-[0.5rem]">
        <div className="flex flex-col gap-[0.5rem]">
          <div className="flex items-center gap-[0.5rem]">
            <LocationIcon className="size-[16px]" />
            <Text variant="bodyBold">{locationAddress}</Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <CalendarIcon className="size-[16px]" />
            <Text variant="bodyBold">{date}</Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <ClockIcon className="size-[16px]" />
            <Text variant="bodyBold">{timeSlot}</Text>
          </div>
          <div className="flex items-center gap-[0.5rem]">
            <PeopleIcon className="size-[16px]" />
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
          className="relative before:absolute before:content[''] before:bottom-0 before:left-0 before:w-full before:h-[1px] before:bg-black"
        >
          Cancel
        </Button>
        <Button variant="secondary" size="l">
          Edit
        </Button>
      </footer>
    </Card>
  );
};
