import { Dish } from "@/types";
import { Button, Text } from "../ui";
import { BinIcon, MinusIcon, PlusIcon } from "../icons";
import { useAppDispatch } from "@/app/hooks";
import {
  decreaseDishInPreorder,
  deleteDishFromPreorder,
  increaseDishInPreorder,
} from "@/app/slices/preordersSlice";
import { cn } from "@/lib/utils";
import { Preorder } from "@/types/preorder.types";

export const CartPreorderDish = ({ dish, className, preorder }: Props) => {
  const dispatch = useAppDispatch();

  return (
    <li className={cn("styleSet-card !rounded @container", className)}>
      <article className="grid grid-rows-[150px_auto] gap-[1rem] @xs:grid-rows-none @xs:grid-cols-[96px_1fr]">
        <div>
          <img
            className="block size-full object-cover"
            // src={dish.imageUrl}
            src="/dish.png"
            width="96px"
            height="96px"
          />
        </div>
        <div className="flex justify-between">
          <div className="flex flex-col">
            <Text variant="bodyBold" tag="h4">
              {dish.name}
            </Text>
            <Text variant="caption" className="mt-[0.5rem]">
              Expected ready time - 10 minutes
            </Text>
            <div className="flex items-center gap-[0.5rem] mt-[1rem] @xs:mt-auto">
              <Button
                variant="secondary"
                size="sm"
                onClick={() =>
                  dispatch(
                    decreaseDishInPreorder({
                      dishId: dish.id,
                      preorderId: preorder.id,
                    }),
                  )
                }
                disabled={dish.count === 1}
              >
                <MinusIcon />
              </Button>
              <Text
                variant="buttonSecondary"
                className="tabular-nums font-sans text-center min-w-[2ch]"
              >
                {dish.count}
              </Text>
              <Button
                variant="secondary"
                size="sm"
                onClick={() =>
                  dispatch(
                    increaseDishInPreorder({
                      dishId: dish.id,
                      preorderId: preorder.id,
                    }),
                  )
                }
              >
                <PlusIcon />
              </Button>
            </div>
          </div>
          <div className="flex flex-col items-end">
            <Button
              variant="tertiary"
              size="sm"
              className="p-0 min-w-auto"
              onClick={() =>
                dispatch(
                  deleteDishFromPreorder({
                    dishId: dish.id,
                    preorderId: preorder.id,
                  }),
                )
              }
            >
              <BinIcon />
            </Button>
            <Text variant="bodyBold" className="mt-auto">
              {Number.parseInt(dish.price) * dish.count} $
            </Text>
          </div>
        </div>
      </article>
    </li>
  );
};

interface Props {
  dish: Dish & { count: number };
  className?: string;
  preorder: Preorder;
}
