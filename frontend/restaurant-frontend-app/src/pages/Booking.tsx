import {
  BookingCard,
  BookingForm,
  Container,
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
        <Text variant="h1" tag="h1" className="text-primary">
          Book a Table
        </Text>
        <BookingForm className="mt-[40px]" />
      </PageHero>
      <Container className="py-[4rem]/[1.875rem]">
        <Text variant="bodyBold" tag="p" className="">
          # tables available
        </Text>
        <ul className="grid gap-[2rem] lg:grid-cols-2">
          <BookingCard></BookingCard>
          <BookingCard></BookingCard>
          <BookingCard></BookingCard>
        </ul>
      </Container>
    </>
  );
};
