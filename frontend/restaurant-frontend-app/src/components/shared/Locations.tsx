import { LocationsCard, Text } from "../ui";
import { Location } from "@/types";
import { PageBody } from "./PageBody";

interface Props {
  locations: Location[];
  isLoading: boolean;
}

export const Locations: React.FC<Props> = ({ locations }) => {
  return (
    <PageBody>
      <Text>Locations</Text>
      <div className="grid grid-cols-[repeat(auto-fit,minmax(250px,1fr))] gap-8">
        {locations.map((item) => (
          <LocationsCard key={item.id} location={item} />
        ))}
      </div>
    </PageBody>
  );
};
