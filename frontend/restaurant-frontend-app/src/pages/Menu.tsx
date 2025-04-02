import { useAppDispatch } from "@/app/hooks";
import { selectDishes } from "@/app/slices/dishesSlice";
import { getAllDishes } from "@/app/thunks/dishesThunks";
import { AllDishes, PageBody, PageHero } from "@/components/shared";
import { Text } from "@/components/ui";
import { useEffect } from "react";
import { useSelector } from "react-redux";

export const Menu = () => {
  const dishes = useSelector(selectDishes);
  const dispatch = useAppDispatch();

  useEffect(() => {
    if (!dishes.length) {
      dispatch(getAllDishes());
    }
  }, [dishes.length, dispatch]);

  return (
    <div>
      <PageHero variant="dark" className="flex flex-col justify-center">
        <Text variant="h2" className="text-primary">
          Green & Tasty Restaurants
        </Text>
        <Text variant="h1" tag="h1" className="text-primary mt-[1.375rem]">
          Menu
        </Text>
      </PageHero>

      <PageBody>
        <AllDishes dishes={dishes} />
      </PageBody>
    </div>
  );
};
