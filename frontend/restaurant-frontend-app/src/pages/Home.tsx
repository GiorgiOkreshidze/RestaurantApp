// import { selectUser } from "@/app/slices/userSlice";
import { Dishes, Hero, Locations } from "@/components/shared";
import { useAppDispatch } from "@/app/hooks";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import { selectPopularDishes } from "@/app/slices/dishesSlice";
import { getPopularDishes } from "@/app/thunks/dishesThunks";
import { selectLocations } from "@/app/slices/locationsSlice";
import { getLocations } from "@/app/thunks/locationsThunks";

export const Home = () => {
  const dispatch = useAppDispatch();
  const popularDishes = useSelector(selectPopularDishes);
  const locations = useSelector(selectLocations);

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
      <Dishes title="Most Popular Dishes" dishes={popularDishes} />
      <Locations locations={locations} />
    </>
  );
};
