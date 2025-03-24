import {
  BookingCard,
  BookingForm,
  PageBody,
  PageHero,
} from "@/components/shared";
import { Text } from "@/components/ui";

export const Booking = () => {
  return (
    <>
      <PageHero variant="dark" className="flex flex-col justify-center">
        <Text variant="h2" className="text-primary">
          Green & Tasty Restaurants
        </Text>
        <Text variant="h1" tag="h1" className="text-primary mt-[1.375rem]">
          Book a Table
        </Text>
        <BookingForm className="mt-[2.5rem]" />
      </PageHero>
      <PageBody>
        <Text># tables available</Text>
        <ul className="grid gap-[2rem] lg:grid-cols-2">
          <BookingCard></BookingCard>
          <BookingCard></BookingCard>
          <BookingCard></BookingCard>
        </ul>
      </PageBody>
    </>
  );
};
