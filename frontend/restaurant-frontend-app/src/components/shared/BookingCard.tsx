import locationThumbnail from "../../assets/images/location-thumbnail.jpg";
import { LocationIcon, PlusIcon } from "../icons";
import { Text } from "../ui";
import { BookingCardButton } from "./BookingCardButton";

export const BookingCard = () => {
  return (
    <li className="@container bg-card-background rounded overflow-hidden shadow-card">
      <article className="grid @max-[650px]:grid-rows-[200px_auto] @[650px]:grid-cols-[200px_1fr]">
        <div className="">
          <img
            className="block object-cover w-full h-full"
            src={locationThumbnail}
          />
        </div>
        <div className="@container p-[1.5rem] flex flex-col gap-[0.75rem]">
          <div className="flex flex-col gap-[1rem]">
            <Text variant="bodyBold" className="flex items-center gap-[0.5rem]">
              <LocationIcon className="size-[16px]" />
              <span>48 Rustaveli Avenue</span>
              <span className="ml-auto">Table #</span>
            </Text>
            <Text variant="bodyBold">Table seating capacity: 4 people</Text>
            <Text variant="bodyBold">7 slots available for Oct 14, 2024:</Text>
          </div>
          <div className="grid gap-[0.5rem] @min-[400px]:grid-cols-2">
            {timeSlots.slice(0, 6).map((timeSlot, i) => (
              <BookingCardButton
                key={i}
                icon={i < 5 ? undefined : <PlusIcon className="size-[1rem]" />}
                className={i < 5 ? undefined : "place-self-start"}
              >
                {i < 5 ? timeSlot : "Shaw all"}
              </BookingCardButton>
            ))}
          </div>
        </div>
      </article>
    </li>
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
