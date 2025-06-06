import { Location } from "@/types/location.types";
import { LocationsCard, Text } from "../ui";
import { PageBodyHeader, PageBodySection } from "./PageBody";

interface Props {
  locations: Location[];
  isLoading: boolean;
}

export const Locations: React.FC<Props> = ({ locations }) => {
  return (
    <PageBodySection>
      <PageBodyHeader>
        <Text variant="h2">Locations</Text>
      </PageBodyHeader>
      <div className="grid grid-cols-[repeat(auto-fit,minmax(250px,1fr))] gap-8">
        {locations.map((item) => (
          <LocationsCard key={item.id} location={item} />
        ))}
      </div>
    </PageBodySection>
  );
};
