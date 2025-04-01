import {
  Dishes,
  PageHero,
  Locations,
  BrandTitle,
  PageBody,
} from "@/components/shared";
import { useSelector } from "react-redux";
import {
  selectDishesLoading,
  selectPopularDishes,
} from "@/app/slices/dishesSlice";
import {
  selectLocations,
  selectLocationsLoading,
} from "@/app/slices/locationsSlice";
import { Button, Text } from "@/components/ui";
import { Link } from "react-router";

export const Home = () => {
  const popularDishes = useSelector(selectPopularDishes);
  const isDishesLoading = useSelector(selectDishesLoading);
  const locations = useSelector(selectLocations);
  const isLocationsLoading = useSelector(selectLocationsLoading);

  return (
    <>
      <PageHero>
        <div className="flex flex-col max-w-[340px]">
          <BrandTitle variant="heroTitle" />
          <Text variant="body" className=" text-neutral-0 mt-[1.5rem]">
            A network of restaurants in Tbilisi, Georgia, offering fresh,
            locally sourced dishes with a focus on health and sustainability.
          </Text>
          <Text variant="body" className=" text-neutral-0 mt-[0.75rem]">
            Our diverse menu includes vegetarian and vegan options, crafted to
            highlight the rich flavors of Georgian cuisine with a modern twist.
          </Text>
          <Link to="/menu">
            <Button className="w-full mt-[2.5rem]">View Menu</Button>
          </Link>
        </div>
      </PageHero>
      <PageBody>
        <Dishes
          title="Most Popular Dishes"
          isLoading={isDishesLoading}
          dishes={popularDishes}
        />
        <Locations isLoading={isLocationsLoading} locations={locations} />
      </PageBody>
    </>
  );
};
