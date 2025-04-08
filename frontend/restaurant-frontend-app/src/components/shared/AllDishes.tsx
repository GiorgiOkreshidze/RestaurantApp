import { DishCard, Text } from "../ui";
import { Dish } from "@/types";
import { OneDishDialog } from "./OneDishDialog";
import { useSelector } from "react-redux";
import { selectOneDish, selectOneDishLoading } from "@/app/slices/dishesSlice";
import { useAppDispatch } from "@/app/hooks";
import { getOneDish } from "@/app/thunks/dishesThunks";
import { useState } from "react";
import { Loader } from "./Loader";

interface Props {
  dishes: Dish[];
  loading: boolean;
}

export const AllDishes: React.FC<Props> = ({ dishes, loading }) => {
  const [isOpen, setIsOpen] = useState(false);
  const oneDish = useSelector(selectOneDish);
  const oneDishLoading = useSelector(selectOneDishLoading);
  const dispatch = useAppDispatch();

  const fetchOneDish = async (id: string) => {
    setIsOpen(true);
    await dispatch(getOneDish(id));
  };

  const handleOpenChange = (open: boolean) => {
    setIsOpen(open);
  };

  const renderContent = () => {
    if (loading) {
      return <Loader />;
    }

    if (dishes.length === 0) {
      return (
        <div className="flex flex-col items-center justify-center">
          <Text variant="h3" className="mb-2">
            No dishes found
          </Text>
          <Text variant="bodyBold">
            Try changing your filters or check back later.
          </Text>
        </div>
      );
    }

    return (
      <>
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
      </>
    );
  };

  return (
    <>
      <div>
        <div className="grid grid-cols-[repeat(auto-fit,minmax(250px,1fr))] gap-8">
          {renderContent()}
        </div>
      </div>
      <OneDishDialog
        dish={oneDish}
        isOpen={isOpen}
        onOpenChange={handleOpenChange}
        loading={oneDishLoading}
      />
    </>
  );
};
