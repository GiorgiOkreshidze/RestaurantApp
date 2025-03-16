// import { selectUser } from "@/app/slices/userSlice";
import { Dishes, Hero, Locations } from "@/components/shared";

import DishImage from "../assets/images/dish.png";
import { Dish } from "@/types";

const mockData: Dish[] = [
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

export const Home = () => {
  return (
    <>
      <Hero />
      <Dishes title="Most Popular Dishes" dishes={mockData} />
      <Locations />
    </>
  );
};
