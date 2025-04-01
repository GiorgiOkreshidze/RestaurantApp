import { DialogTitle } from "@radix-ui/react-dialog";
import { Dialog, DialogContent, Text } from "../ui";
import { ExtendedDish } from "@/types";

interface Props {
  dish: ExtendedDish | null;
  isOpen: boolean;
  onOpenChange: (open: boolean) => void;
}

export const OneDishDialog: React.FC<Props> = ({
  isOpen,
  dish,
  onOpenChange,
}) => {
  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange}>
      <DialogTitle></DialogTitle>
      <DialogContent>
        {dish && (
          <>
            <img
              src={dish.imageUrl}
              alt={dish.name}
              className="w-[300px] h-[300px] block mx-auto"
            />
            <Text variant="h3">{dish.name}</Text>
            <Text>{dish.description}</Text>
            <Text>
              <b>Calories: </b>
              {dish.calories}
            </Text>

            <div>
              <Text>
                <b>Proteins: </b>
                {dish.proteins}
              </Text>
              <Text>
                <b>Fats: </b>
                {dish.fats}
              </Text>
              <Text>
                <b>Carbohydrates: </b>
                {dish.carbohydrates}
              </Text>
            </div>

            <div className="flex items-center justify-between">
              <Text variant="h3">{dish.price}</Text>
              <Text variant="h3">{dish.weight}</Text>
            </div>
          </>
        )}
      </DialogContent>
    </Dialog>
  );
};
