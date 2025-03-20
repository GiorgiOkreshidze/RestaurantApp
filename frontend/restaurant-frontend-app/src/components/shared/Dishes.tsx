import { Container } from "./container";
import { DishCard, Text } from "../ui";
import { Dish } from "@/types";

interface Props {
  title: string;
  isLoading: boolean;
  dishes: Dish[];
}

export const Dishes: React.FC<Props> = ({
  isLoading = false,
  title,
  dishes,
}) => {
  return (
    <div>
      <Container className="!py-[64px]">
        <Text variant="h2" className="mb-10">
          {title}
        </Text>
        <div className="grid grid-cols-[repeat(auto-fit,minmax(250px,1fr))] gap-8">
          {/* <div className="flex flex-wrap gap-8"> */}
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
