import { CategoryType } from "@/types";
import { useState } from "react";

export const useFilterState = () => {
  const [filters, setFilters] = useState<{
    activeCategory: CategoryType;
    sortBy: string;
  }>({
    activeCategory: "",
    sortBy: "",
  });

  const handleCategoryToggle = (category: CategoryType) => {
    setFilters((prev) => ({
      ...prev,
      activeCategory: prev.activeCategory === category ? "" : category,
    }));
  };

  const isCategoryActive = (category: CategoryType) =>
    filters.activeCategory === category;

  const setSort = (sortBy: string) => {
    setFilters((prev) => ({
      ...prev,
      sortBy,
    }));
  };

  return { filters, handleCategoryToggle, isCategoryActive, setSort };
};
