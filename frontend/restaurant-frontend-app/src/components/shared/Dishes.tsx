import { DishCard, Text } from "../ui";
import type { Dish } from "@/types";
import { PageBodyHeader, PageBodySection } from "./PageBody";

interface Props {
  isLoading?: boolean;
  dishes: Dish[];
  title: string;
}

export const Dishes: React.FC<Props> = ({ dishes, title }) => {
  return (
    <PageBodySection>
      <PageBodyHeader>
        <Text variant="h2">{title}</Text>
      </PageBodyHeader>
      <div className="grid grid-cols-[repeat(auto-fit,minmax(250px,1fr))] gap-8">
        {dishes.slice(0, 4).map((item) => (
          <DishCard
            key={item.id}
            name={item.name}
            price={item.price}
            weight={item.weight}
            imageUrl={item.imageUrl}
          />
        ))}
      </div>
    </PageBodySection>
  );
};
