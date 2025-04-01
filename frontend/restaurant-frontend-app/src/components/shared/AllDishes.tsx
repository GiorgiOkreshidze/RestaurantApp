import { DishCard } from "../ui";
import { Dish } from "@/types";
import { OneDishDialog } from "./OneDishDialog";
import { useSelector } from "react-redux";
import { selectOneDish } from "@/app/slices/dishesSlice";
import { useAppDispatch } from "@/app/hooks";
import { getOneDish } from "@/app/thunks/dishesThunks";
import { useState } from "react";

interface Props {
  dishes: Dish[];
}

export const AllDishes: React.FC<Props> = ({ dishes }) => {
  const [isOpen, setIsOpen] = useState(false);
  const oneDish = useSelector(selectOneDish);
  const dispatch = useAppDispatch();

  const fetchOneDish = async (id: string) => {
    await dispatch(getOneDish(id));
    setIsOpen(true);
  };

  const handleOpenChange = (open: boolean) => {
    setIsOpen(open);
  };

  return (
    <>
      <div>
        <div className="grid grid-cols-[repeat(auto-fit,minmax(250px,1fr))] gap-8">
          {dishes.map((dish) => (
            <DishCard
              key={dish.id}
              name={dish.name}
              price={dish.price}
              weight={dish.weight}
              imageUrl={dish.imageUrl}
              state={dish.state}
              onClick={() => fetchOneDish(dish.id)}
            />
          ))}
        </div>
      </div>
      <OneDishDialog
        dish={oneDish}
        isOpen={isOpen}
        onOpenChange={handleOpenChange}
      />
    </>
  );
};
