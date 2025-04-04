import { DialogTitle } from "@radix-ui/react-dialog";
import { Dialog, DialogContent, Text } from "../ui";
import { ExtendedDish } from "@/types";
import { Loader } from "./Loader";

interface Props {
  dish: ExtendedDish | null;
  isOpen: boolean;
  onOpenChange: (open: boolean) => void;
  loading: boolean;
}

export const OneDishDialog: React.FC<Props> = ({
  isOpen,
  dish,
  onOpenChange,
  loading,
}) => {
  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange}>
      <DialogTitle></DialogTitle>
      <DialogContent className="gap-0">
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
          </>
        )}
      </DialogContent>
    </Dialog>
  );
};
