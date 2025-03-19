import { Container } from "./container";
import { DishCard, Text } from "../ui";
import { Dish } from "@/types";

interface Props {
  title: string;
  dishes: Dish[];
}

export const Dishes: React.FC<Props> = ({ title, dishes }) => {
  return (
    <div>
      <Container className="!py-[64px]">
        <Text variant="h2" className="mb-10">
          {title}
        </Text>
        <div className="flex gap-8">
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
      </Container>
    </div>
  );
};
