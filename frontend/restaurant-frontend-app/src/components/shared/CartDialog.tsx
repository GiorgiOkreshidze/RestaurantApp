import {
  DialogContent,
  Dialog,
  DialogHeader,
  DialogTitle,
  Text,
  CustomLink,
} from "../ui";
import {
  selectIsCartDialogOpen,
  setIsCartDialogOpen,
} from "@/app/slices/cartSlice";
import { useSelector } from "react-redux";
import { useAppDispatch } from "@/app/hooks";
import { selectPreorders } from "@/app/slices/preordersSlice";
import { CartPreorder } from "./CartPreorder";
import { Link } from "react-router";

export const CartDialog = () => {
  const dispatch = useAppDispatch();
  const isOpen = useSelector(selectIsCartDialogOpen);
  const preorders = useSelector(selectPreorders);
  const submittedPreorders = preorders.filter(
    (preorder) => preorder.status === "submitted",
  );
  const notSubmittedPreorders = preorders.filter(
    (preorder) => preorder.status === "new" && preorder.dishes.length > 0,
  );

  return (
    <Dialog
      open={isOpen}
      onOpenChange={(state) => dispatch(setIsCartDialogOpen(state))}
    >
      <DialogContent className="content-start min-h-screen top-0 left-auto right-0 rounded-none translate-none data-[state=open]:slide-in-from-right data-[state=open]:zoom-in-100 data-[state=open]:fade-in-100 data-[state=closed]:slide-out-to-right data-[state=closed]:fade-out-100 data-[state=closed]:zoom-out-100 max-w-none sm:max-w-[min(640px,100vw)]">
        <DialogHeader>
          <DialogTitle asChild>
            <Text variant="h2" tag="h2" className="text-center">
              Cart
            </Text>
          </DialogTitle>
        </DialogHeader>
        {notSubmittedPreorders.length > 0 && (
          <div>
            <Text variant="h3" tag="h3">
              Not submitted pre-orders
            </Text>
            <ul className="grid gap-[2.5rem] mt-[1.5rem]">
              {notSubmittedPreorders.map((preorder) => (
                <CartPreorder key={preorder.id} preorder={preorder} />
              ))}
            </ul>
          </div>
        )}
        {submittedPreorders.length > 0 && (
          <div>
            <Text variant="h3" tag="h3">
              Submitted pre-orders
            </Text>
            <Text variant="body">
              You can modify or cancel your pre-order up to 30 minutes before
              your reserved table time.
            </Text>
            <ul className="grid gap-[2.5rem] mt-[1.5rem]">
              {submittedPreorders.map((preorder) => (
                <CartPreorder key={preorder.id} preorder={preorder} />
              ))}
            </ul>
          </div>
        )}
        {preorders.length === 0 && (
          <Text variant="body">
            There are no preorders. Make one on the{" "}
            <CustomLink asChild>
              <Link
                to="/reservations"
                onClick={() => dispatch(setIsCartDialogOpen(false))}
              >
                Reservations
              </Link>
            </CustomLink>{" "}
            page
          </Text>
        )}
      </DialogContent>
    </Dialog>
  );
};
