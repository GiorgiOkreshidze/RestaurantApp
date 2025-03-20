import { LocationsCard, Text } from "../ui";
import { Container } from "./container";
import { Location } from "@/types";

interface Props {
  locations: Location[];
  isLoading: boolean;
}

export const Locations: React.FC<Props> = ({ locations }) => {
  return (
    <div>
      <Container className="pb-[64px] !pt-0">
        <Text variant="h2" className="mb-10">
          Locations
        </Text>
        <div className="grid grid-cols-[repeat(auto-fit,minmax(250px,1fr))] gap-8">
          {locations.map((item) => (
            <LocationsCard key={item.id} location={item} />
          ))}
        </div>
      </Container>
    </div>
  );
};
