import {
  BookingCard,
  BookingForm,
  PageBody,
  PageBodyHeader,
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
        <PageBodyHeader>
          <Text variant="bodyBold"># tables available</Text>
        </PageBodyHeader>
        <ul className="grid gap-[2rem] lg:grid-cols-2">
          <BookingCard></BookingCard>
          <BookingCard></BookingCard>
          <BookingCard></BookingCard>
        </ul>
      </PageBody>
    </>
  );
};
