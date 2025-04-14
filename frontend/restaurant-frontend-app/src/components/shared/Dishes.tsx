import { DishCard, Text } from "../ui";
import type { Dish } from "@/types";
import { PageBodyHeader, PageBodySection } from "./PageBody";
import { useState } from "react";
import { useSelector } from "react-redux";
import { selectOneDish, selectOneDishLoading } from "@/app/slices/dishesSlice";
import { useAppDispatch } from "@/app/hooks";
import { OneDishDialog } from "./OneDishDialog";
import { getOneDish } from "@/app/thunks/dishesThunks";
import { selectActivePreorder } from "@/app/slices/preordersSlice";

interface Props {
  isLoading?: boolean;
  dishes: Dish[];
  title: string;
}

export const Dishes: React.FC<Props> = ({ dishes, title }) => {
  const [isOpen, setIsOpen] = useState(false);
  const oneDish = useSelector(selectOneDish);
  const oneDishLoading = useSelector(selectOneDishLoading);
  const dispatch = useAppDispatch();
  const activePreorder = useSelector(selectActivePreorder);

  const fetchOneDish = async (id: string) => {
    setIsOpen(true);
    await dispatch(getOneDish(id));
  };

  const handleOpenChange = (open: boolean) => {
    setIsOpen(open);
  };

  return (
    <PageBodySection>
      <PageBodyHeader>
        <Text variant="h2">{title}</Text>
      </PageBodyHeader>
      <div className="grid grid-cols-[repeat(auto-fill,minmax(250px,1fr))] gap-8">
        {dishes.slice(0, 4).map((item) => (
          <DishCard
            key={item.id}
            id={item.id}
            name={item.name}
            price={item.price}
            weight={item.weight}
            imageUrl={item.imageUrl}
            onClick={() => fetchOneDish(item.id)}
          />
        ))}
      </div>
      <OneDishDialog
        dish={oneDish}
        isOpen={isOpen}
        onOpenChange={handleOpenChange}
        loading={oneDishLoading}
      />
    </PageBodySection>
  );
};
