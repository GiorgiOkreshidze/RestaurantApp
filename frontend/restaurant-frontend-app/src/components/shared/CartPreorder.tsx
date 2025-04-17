import { Preorder } from "@/types/preorder.types";
import { Button, Text } from "../ui";
import { useSelector } from "react-redux";
import { selectReservations } from "@/app/slices/reservationsSlice";
import { formatDateToUI, formatTimeToUI } from "@/utils/dateTime";
import { selectDishes } from "@/app/slices/dishesSlice";
import { CartPreorderDish } from ".";
import { PlusIcon } from "../icons";
import { useAppDispatch } from "@/app/hooks";
import { setIsCartDialogOpen } from "@/app/slices/cartSlice";
import { useLocation, useNavigate } from "react-router";
import {
  deletePreorder,
  setActivePreorder,
  setPreorderStatus,
} from "@/app/slices/preordersSlice";

export const CartPreorder = ({ preorder, className }: Props) => {
  const dispatch = useAppDispatch();
  const location = useLocation();
  const navigate = useNavigate();
  const allReservations = useSelector(selectReservations);
  const allDishes = useSelector(selectDishes);
  const reservation = allReservations.find(
    (reservation) => reservation.id === preorder.id,
  );
  const preorderDishes = preorder.dishes
    .map((preorderDish) => {
      const dish = allDishes.find((dish) => dish.id === preorderDish.id);
      if (!dish) return undefined;
      return {
        ...dish,
        count: preorderDish.count,
      };
    })
    .filter((item) => item !== undefined);
    console.log( preorderDishes )
  const handleAddDishesClick = () => {
    dispatch(setActivePreorder(preorder.id));
    dispatch(setIsCartDialogOpen(false));
    if (location.pathname !== "/menu") {
      navigate("/menu");
    }
  };

  const totalPayment = preorderDishes
    .map((dish) => dish.count * Number.parseInt(dish.price))
    .reduce((sum, value) => sum + value, 0);

  const handleSubmitPreorder = () => {
    dispatch(
      setPreorderStatus({ preorderId: preorder.id, status: "submitted" }),
    );
  };

  const handleEditPreorder = () => {
    dispatch(setPreorderStatus({ preorderId: preorder.id, status: "new" }));
  };

  const handleCancelPreorder = () => {
    dispatch(deletePreorder(preorder.id));
  }

  if (!reservation) return null;

  return (
    <li className={className}>
      <Text variant="bodyBold">
        Pre-order #{preorder.number} ({reservation.locationAddress},{" "}
        {reservation.tableNumber}, {formatDateToUI(reservation.date)},{" "}
        {formatTimeToUI(reservation.timeFrom)} -{" "}
        {formatTimeToUI(reservation.timeTo)})
      </Text>
      {preorder.status === "new" && (
        <ul className="flex flex-col gap-[0.5rem] mt-[1rem]">
          {preorderDishes.map((dish) => (
            <CartPreorderDish key={dish.id} dish={dish} preorder={preorder} />
          ))}
        </ul>
      )}
      {preorder.status === "submitted" && (
        <ul className="flex flex-col gap-[0.5rem] mt-[1rem]">
          {preorderDishes.map((dish) => (
            <li key={dish.id} className="flex items-center justify-between">
              <Text variant="body">
                {dish.count}x {dish.name}
              </Text>
              <Text variant="bodyBold">{dish.price} $</Text>
            </li>
          ))}
        </ul>
      )}
      {preorder.status === "new" && (
        <Button
          variant="secondary"
          size="l"
          className="w-full mt-[0.5rem]"
          onClick={handleAddDishesClick}
        >
          <PlusIcon />
          <span>Add Dishes</span>
        </Button>
      )}
      <div className="flex items-center justify-end gap-[1rem] mt-[0.5rem]">
        <Text variant="bodyBold">Total Payment</Text>
        <Text variant="h3">{totalPayment} $</Text>
      </div>
      {preorder.status === "new" && (
        <Button
          variant="primary"
          size="l"
          className="w-full mt-[1rem]"
          onClick={handleSubmitPreorder}
        >
          Submit Pre-order
        </Button>
      )}
      {preorder.status === "submitted" && (
        <div className="flex items-center gap-[1rem] justify-end mt-[1rem]">
          <Button variant="underlined" size="l" onClick={handleCancelPreorder}>
            Cancel Pre-order
          </Button>
          <Button variant="secondary" size="l" onClick={handleEditPreorder}>
            Edit Pre-order
          </Button>
        </div>
      )}
    </li>
  );
};

interface Props {
  preorder: Preorder;
  className?: string;
}
