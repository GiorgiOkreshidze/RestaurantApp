import { DialogTitle } from "@radix-ui/react-dialog";
import { Button, Dialog, DialogContent, DialogDescription, Text } from "../ui";
import { Dish, ExtendedDish } from "@/types";
import { Loader } from "./Loader";
import { Preorder } from "@/types/preorder.types";

interface Props {
  dish: ExtendedDish | null;
  isOpen: boolean;
  onOpenChange: (open: boolean) => void;
  loading: boolean;
  activePreorder?: Preorder | undefined;
  onPreorderClick?: (isPreordered: boolean, dishId: Dish["id"]) => void;
}

export const OneDishDialog: React.FC<Props> = ({
  isOpen,
  dish,
  loading,
  onOpenChange,
  activePreorder,
  onPreorderClick,
}) => {
  const isPreordered = activePreorder?.dishes.find(
    (arrayDish) => arrayDish.id === dish?.id,
  );

  return (
    <Dialog
      open={isOpen}
      onOpenChange={onOpenChange}
      
    >
      <DialogTitle></DialogTitle>
      <DialogDescription className="sr-only">
        Detailed information about the selected dish
      </DialogDescription>
      <DialogContent className="gap-0" data-testid="one-dish-dialog" >
        {loading ? (
          <Loader />
        ) : (
          <>
            <img
              src={dish?.imageUrl}
              alt={dish?.name}
              className="w-[300px] h-[300px] block mx-auto"
            />
            <Text variant="h3" className="my-6">
              {dish?.name}
            </Text>
            <Text className="mb-3">{dish?.description}</Text>
            <Text className="mb-3">
              <b>Calories: </b>
              {dish?.calories}
            </Text>

            <div className="mb-3">
              <Text>
                <b>Proteins: </b>
                {dish?.proteins}
              </Text>
              <Text>
                <b>Fats: </b>
                {dish?.fats}
              </Text>
              <Text>
                <b>Carbohydrates: </b>
                {dish?.carbohydrates}
              </Text>
            </div>

            <Text className="mb-6">
              <b>Vitamins and minerals:</b> {dish?.vitamins}
            </Text>

            <div className="flex items-center justify-between">
              <Text variant="h3">{dish?.price}</Text>
              <Text variant="h3">{dish?.weight}</Text>
            </div>

            {activePreorder && (
              <Button
                variant={isPreordered ? "secondary" : "primary"}
                size="l"
                className="w-full mt-[1.5rem]"
                onClick={(e) => {
                  if (!onPreorderClick || !dish) return;
                  e.stopPropagation();
                  onPreorderClick(Boolean(isPreordered), dish.id);
                }}
              >
                {isPreordered ? "In Cart" : "Pre-order"}
              </Button>
            )}
          </>
        )}
      </DialogContent>
    </Dialog>
  );
};
