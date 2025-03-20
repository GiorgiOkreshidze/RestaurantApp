import { Plus } from "lucide-react";
import locationThumbnail from "../../assets/images/location-thumbnail.jpg";
import { ClockIcon, LocationIcon } from "../icons";
import { Button, Text } from "../ui";

export const BookingCard = () => {
  return (
    <article className="bg-card-background rounded">
      <img className="" src={locationThumbnail} />
      <div>
        <Text variant="bodyBold" className="flex items-center gap-[0.5rem]">
          <LocationIcon className="size-[16px]" /> 48 Rustaveli Avenue
        </Text>
        <Text variant="bodyBold" className="mt-[1rem]">
          Table seating capacity: 4 people
        </Text>
        <Text variant="bodyBold" className="mt-[1rem]">
          7 slots available for Oct 14, 2024:
        </Text>
      </div>
      <div className="flex flex-wrap gap-[0.5rem]">
        {timeSlots.slice(0, 5).map((timeSlot, i) => (
          <Button
            key={i}
            variant="secondary"
            size="sm"
            className="grow-1 flex gap-[0.5rem]"
            icon={<ClockIcon />}
          >
            {timeSlot}
          </Button>
        ))}
        {timeSlots.length > 5 && (
          <Button
            variant="secondary"
            size="sm"
            className="grow-1 flex gap-[0.5rem]"
          >
            <Plus />
            Show all
          </Button>
        )}
      </div>
    </article>
  );
};

const timeSlots = [
  "10:30 a.m. - 12:00 p.m",
  "12:15 p.m. - 1:45 p.m",
  "2:00 p.m. - 3:30 p.m",
  "3:45 p.m. - 5:15 p.m",
  "5:30 p.m. - 7:00 p.m",
  "8:30 p.m. - 9:30 p.m",
];
