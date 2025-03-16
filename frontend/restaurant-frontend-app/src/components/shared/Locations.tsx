import { Container } from "./Container";
import { LocationsCard, Text } from "../ui";
import LocationImage from "../../assets/images/location.jpg";

const mockData = [
  {
    name: "48 Rustaveli Avenue",
    capacity: 10,
    occupancy: 90,
    image: LocationImage,
  },
  {
    name: "48 Rustaveli Avenue",
    capacity: 10,
    occupancy: 90,
    image: LocationImage,
  },
  {
    name: "48 Rustaveli Avenue",
    capacity: 10,
    occupancy: 90,
    image: LocationImage,
  },
];

export const Locations = () => {
  return (
    <div>
      <Container className="pb-[64px] pt-0">
        <Text variant="h1" className="mb-10">
          Locations
        </Text>
        <div className="flex gap-8">
          {mockData.map((item, index) => (
            <LocationsCard
              key={index}
              name={item.name}
              capacity={item.capacity}
              occupancy={item.occupancy}
              image={item.image}
            />
          ))}
        </div>
      </Container>
    </div>
  );
};
