import {
  BookingCard,
  BookingForm,
  Container,
  HeroWrapper,
} from "@/components/shared";
import { Text } from "@/components/ui";

export const Booking = () => {
  return (
    <>
      <HeroWrapper variant="dark" className="flex flex-col justify-center">
        <Text variant="h2" className="text-primary">
          Green & Tasty Restaurants
        </Text>
        <Text variant="h1" tag="h1" className="text-primary">
          Book a Table
        </Text>
        <BookingForm className="mt-[40px]" />
      </HeroWrapper>
      <Container>{/* <BookingCard></BookingCard> */}</Container>
    </>
  );
};
