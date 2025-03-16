import { Container } from "./Container";
import { DishCard, Text } from "../ui";
import DishImage from "../../assets/images/dish.png";

const mockData = [
  {
    name: "Fresh Strawberry Mint Salad",
    cost: "17",
    weight: "430",
    image: DishImage,
  },
  {
    name: "Fresh Strawberry Mint Salad",
    cost: "17",
    weight: "430",
    image: DishImage,
  },
  {
    name: "Fresh Strawberry Mint Salad",
    cost: "17",
    weight: "430",
    image: DishImage,
  },
  {
    name: "Fresh Strawberry Mint Salad",
    cost: "17",
    weight: "430",
    image: DishImage,
  },
];

export const Dishes = () => {
  return (
    <div>
      <Container className="py-[64px]">
        <Text variant="h1" className="mb-10">
          Most Popular Dishes
        </Text>
        <div className="flex gap-8">
          {mockData.map((item, index) => (
            <DishCard
              key={index}
              name={item.name}
              cost={item.cost}
              weight={item.weight}
              image={item.image}
            />
          ))}
        </div>
      </Container>
    </div>
  );
};
