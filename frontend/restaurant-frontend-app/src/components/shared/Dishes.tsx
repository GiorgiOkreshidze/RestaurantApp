import { DishCard, Text } from "../ui";
import { Dish } from "@/types";
import { PageBody } from "./PageBody";

interface Props {
  title: string;
  isLoading?: boolean;
  dishes: Dish[];
}

export const Dishes: React.FC<Props> = ({ title, dishes }) => {
  return (
    <PageBody>
      <Text>{title}</Text>
      <div className="grid grid-cols-[repeat(auto-fit,minmax(250px,1fr))] gap-8">
        {dishes.slice(0, 4).map((item, index) => (
          <DishCard
            key={index}
            name={item.name}
            price={item.price}
            weight={item.weight}
            imageUrl={item.imageUrl}
          />
        ))}
      </div>
    </PageBody>
  );
};
