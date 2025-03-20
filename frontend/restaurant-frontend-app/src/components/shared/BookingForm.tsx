import { ChevronDown } from "lucide-react";
import { ClockIcon, LocationIcon, PeopleIcon } from "../icons";
import { Button, Form, Text } from "../ui";
import {
  DialogContent,
  DialogTitle,
  DialogTrigger,
  Dialog,
  DialogDescription,
  DialogHeader,
} from "../ui/dialog";
import {
  SelectTrigger,
  Select,
  SelectContent,
  SelectValue,
  SelectItem,
} from "../ui/Select";
import { CalendarField } from "./CalendarField";
import { ComponentProps } from "react";
import { cn } from "@/lib/utils";

export const BookingForm = ({
  className,
  ...props
}: ComponentProps<"form">) => {
  return (
    <Form>
      <form
        className={cn(
          "flex items-center gap-[1rem] flex-wrap *:grow-1",
          className,
        )}
        {...props}
      >
        <Select>
          <SelectTrigger className="place-items-start grow-1 grid grid-cols-[auto_1fr_auto]">
            <LocationIcon
              color="var(--color-foreground)"
              className="size-[24px]"
            />
            <SelectValue placeholder="Location" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="1">48 Rustaveli Avenue</SelectItem>
            <SelectItem value="2">14 Baratashvili Street</SelectItem>
            <SelectItem value="3">9 Abashidze Street</SelectItem>
          </SelectContent>
        </Select>
        <CalendarField />
        <Dialog>
          <DialogTrigger asChild>
            <Button
              variant="secondary"
              className="grow-1 flex items-center justify-start gap-[0.5rem] justify-starttext-foreground fontset-buttonSecondary"
            >
              <ClockIcon className="size-[24px]" />
              <Text variant="buttonSecondary">Time</Text>
              <ChevronDown className="text-foreground ml-auto" />
            </Button>
          </DialogTrigger>
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
        <div className="flex gap-[1rem] items-center py-[0.75rem] px-[1.5rem] bg-card-background rounded">
          <PeopleIcon className="size-[24px]" />
          <Text variant="buttonSecondary">Guests</Text>
          <Button variant="secondary" size="sm" className="min-w-[40px]">
            -
          </Button>
          <span>10</span>
          <Button variant="secondary" size="sm" className=" min-w-[40px]">
            +
          </Button>
        </div>
        <Button className="grow-1">Find&nbsp;a&nbsp;Table</Button>
      </form>
    </Form>
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
