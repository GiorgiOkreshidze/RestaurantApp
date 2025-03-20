import { Container, HeroCommon } from "@/components/shared";
import { Text } from "@/components/ui";

export const Booking = () => {
  return (
    <>
      <HeroCommon variant="dark">
        <Text variant="h2" className="text-primary">
          Green & Tasty Restaurants
        </Text>
        <Text variant="h1" tag="h1" className="text-primary">
          Book a Table
        </Text>
      </HeroCommon>
      <Container>
        <h1>Hello</h1>
      </Container>
    </>
  );
};
