// import { selectUser } from "@/app/slices/userSlice";
import { Dishes, PageHero, Locations, Title } from "@/components/shared";
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

export const Home = () => {
  const popularDishes = useSelector(selectPopularDishes);
  const isDishesLoading = useSelector(selectDishesLoading);
  const locations = useSelector(selectLocations);
  const isLocationsLoading = useSelector(selectLocationsLoading);

  return (
    <>
      <PageHero>
        <div className="max-w-[340px]">
          <Title variant="navBarLogo" className="text-green-200 !text-5xl" />
          <Text variant="body" className=" text-neutral-0 mt-6 mb-3">
            A network of restaurants in Tbilisi, Georgia, offering fresh,
            locally sourced dishes with a focus on health and sustainability.
          </Text>
          <Text variant="body" className=" text-neutral-0 mb-10">
            Our diverse menu includes vegetarian and vegan options, crafted to
            highlight the rich flavors of Georgian cuisine with a modern twist.
          </Text>
          <Button className="w-full">View Menu</Button>
        </div>
      </PageHero>
      <Dishes
        isLoading={isDishesLoading}
        title="Most Popular Dishes"
        dishes={popularDishes}
      />
      <Locations isLoading={isLocationsLoading} locations={locations} />
    </>
  );
};
