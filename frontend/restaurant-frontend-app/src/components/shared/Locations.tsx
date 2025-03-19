import { LocationsCard, Text } from "../ui";
import { Container } from "./container";
import { Location } from "@/types";

interface Props {
  locations: Location[];
}

export const Locations: React.FC<Props> = ({ locations }) => {
  return (
    <div>
      <Container className="pb-[64px] !pt-0">
        <Text variant="h2" className="mb-10">
          Locations
        </Text>
        <div className="flex gap-8">
          {locations.map((item) => (
            <LocationsCard key={item.id} location={item} />
          ))}
        </div>
      </Container>
    </div>
  );
};
