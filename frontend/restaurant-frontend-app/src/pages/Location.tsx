import { Container, Dishes, LocationHero, Reviews } from "@/components/shared";
import { Text } from "@/components/ui";
import { NavLink, useParams } from "react-router";
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

export const Location = () => {
  const { id } = useParams();

  return (
    <>
      {/* Breadcrumbs, пока что захардкоженный */}
      <Container className="py-0">
        <div className="mb-8 flex items-center">
          <NavLink to={"/"}>
            <Text variant="caption">Main Page &gt;</Text>
          </NavLink>
          <NavLink to={`locations/${id}`}>Location 48 Rustaveli</NavLink>
        </div>
      </Container>

      <LocationHero />

      <Dishes title="Specialty Dishes" dishes={mockData} />

      <Reviews />
    </>
  );
};
