// import { selectUser } from "@/app/slices/userSlice";
import { Dishes, Hero, Locations } from "@/components/shared";
import { useAppDispatch } from "@/app/hooks";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import {
  selectDishesLoading,
  selectPopularDishes,
} from "@/app/slices/dishesSlice";
import { getPopularDishes } from "@/app/thunks/dishesThunks";
import {
  selectLocations,
  selectLocationsLoading,
} from "@/app/slices/locationsSlice";
import { getLocations } from "@/app/thunks/locationsThunks";

export const Home = () => {
  const dispatch = useAppDispatch();
  const popularDishes = useSelector(selectPopularDishes);
  const isDishesLoading = useSelector(selectDishesLoading);
  const locations = useSelector(selectLocations);
  const isLocationsLoading = useSelector(selectLocationsLoading);

  useEffect(() => {
    if (!popularDishes.length) {
      dispatch(getPopularDishes());
    }
  }, [dispatch, popularDishes.length]);

  useEffect(() => {
    if (!locations.length) {
      dispatch(getLocations());
    }
  }, [dispatch, locations.length]);

  return (
    <>
      <Hero />
      <Dishes
        isLoading={isDishesLoading}
        title="Most Popular Dishes"
        dishes={popularDishes}
      />
      <Locations isLoading={isLocationsLoading} locations={locations} />
    </>
  );
};
