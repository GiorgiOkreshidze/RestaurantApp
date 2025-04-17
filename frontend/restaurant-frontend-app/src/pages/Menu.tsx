import { useAppDispatch } from "@/app/hooks";
import { selectDishes, selectDishesLoading } from "@/app/slices/dishesSlice";
import { getAllDishes } from "@/app/thunks/dishesThunks";
import {
  AllDishes,
  PageBody,
  PageHero,
  PreorderInfoBar,
} from "@/components/shared";
import { CategoryFilters } from "@/components/shared/CategoryFilters";
import { SortingOptions } from "@/components/shared/SortingOptions";
import { Text } from "@/components/ui";
import { useFilterState } from "@/hooks/useFiltersState";
import { CategoryType, SortOptionType } from "@/types";
import { useEffect, useMemo } from "react";
import { useSelector } from "react-redux";

export const Menu = () => {
  const dishes = useSelector(selectDishes);
  const dispatch = useAppDispatch();
  const isLoading = useSelector(selectDishesLoading);

  const { filters, handleCategoryToggle, isCategoryActive, setSort } =
    useFilterState();

  useEffect(() => {
    dispatch(
      getAllDishes({
        category: filters.activeCategory,
        sortBy: filters.sortBy,
      })
    );
  }, [filters.activeCategory, filters.sortBy, dispatch]);

  const categories: CategoryType[] = ["Appetizers", "MainCourse", "Desserts"];

  const sortOptions: SortOptionType[] = useMemo(
    () => [
      { id: "PopularityAsc", label: "Popularity Ascending" },
      { id: "PopularityDesc", label: "Popularity Descending" },
      { id: "PriceAsc", label: "Price Ascending" },
      { id: "PriceDesc", label: "Price Descending" },
    ],
    []
  );

  return (
    <div>
      <PreorderInfoBar />
      <PageHero variant="dark" className="flex flex-col justify-center">
        <Text variant="h2" className="text-primary">
          Green & Tasty Restaurants
        </Text>
        <Text variant="h1" tag="h1" className="text-primary mt-[1.375rem]">
          Menu
        </Text>
      </PageHero>

      <PageBody>
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 sm:gap-0 mb-10">
          <CategoryFilters
            categories={categories}
            handleCategoryToggle={handleCategoryToggle}
            isCategoryActive={isCategoryActive}
          />
          <SortingOptions
            options={sortOptions}
            value={filters.sortBy}
            onChange={setSort}
          />
        </div>

        <AllDishes dishes={dishes} loading={isLoading} />
      </PageBody>
    </div>
  );
};
