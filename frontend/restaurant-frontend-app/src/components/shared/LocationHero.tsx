import { Container, Title } from ".";
import { LocationIcon, StarIcon } from "../icons";
import { Button, Text } from "../ui";

import LocationImage from "../../assets/images/location.jpg";

export const LocationHero = () => {
  return (
    <Container>
      <div className="flex gap-20 justify-between items-center">
        <div className="max-w-[340px]">
          <Title variant="navBarLogo" className="text-green-200 !text-5xl mb-6" />

          <div className="flex items-center justify-between mb-6">
            <div className="flex gap-2.5">
              <LocationIcon className="stroke-green-200" />
              <Text variant="bodyBold" className="">
                48 Rustaveli Avenue
              </Text>
            </div>
            <div className="flex gap-1 items-center">
              <Text>4.73</Text>
              <StarIcon />
            </div>
          </div>

          <div>
            <Text variant="blockTitle" className="mb-3">
              Located on bustling Rustaveli Avenue, this branch offers a perfect
              mix of city energy and a cozy atmosphere.
            </Text>
            <Text variant="blockTitle" className="mb-3">
              Known for our fresh, locally sourced dishes, we focus on health
              and sustainability, featuring Georgian cuisine with a modern
              twist. The menu includes vegetarian and vegan options, along with
              exclusive seasonal specials.
            </Text>
            <Text variant="blockTitle" className="mb-10">
              With its spacious outdoor terrace, this location is ideal for both
              casual lunches and intimate dinners.
            </Text>
          </div>

          <Button>Book a Table</Button>
        </div>
        <div
          className="w-full h-[500px] rounded-3xl bg-cover bg-center"
          style={{ backgroundImage: `url(${LocationImage})` }}
        ></div>
      </div>
    </Container>
  );
};
