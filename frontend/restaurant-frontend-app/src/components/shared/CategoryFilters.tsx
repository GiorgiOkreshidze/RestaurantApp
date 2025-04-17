import { CategoryType } from "@/types";
import { Button } from "../ui";

export const CategoryFilters = ({
  categories,
  handleCategoryToggle,
  isCategoryActive,
}: {
  categories: CategoryType[];
  handleCategoryToggle: (category: CategoryType) => void;
  isCategoryActive: (category: CategoryType) => boolean;
}) => (
  <div className="flex items-center gap-4 flex-wrap">
    {categories.map((category) => (
      <Button
        key={category}
        onClick={() => handleCategoryToggle(category)}
        variant={isCategoryActive(category) ? "primary" : "secondary"}
        size="sm"
        className="min-w-[135px]"
      >
        {category === "MainCourse" ? "Main Courses" : category}
      </Button>
    ))}
  </div>
);
