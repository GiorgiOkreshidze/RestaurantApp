import { useAppDispatch } from "@/app/hooks";
import {
  selectLocationTables,
  selectSelectOptions,
  selectSelectOptionsLoading,
} from "@/app/slices/locationsSlice";
import { selectReservationCreatingLoading } from "@/app/slices/reservationsSlice";
import {
  selectUser,
  selectAllUsers,
  selectAllUsersLoading,
} from "@/app/slices/userSlice";
import { upsertWaiterReservation } from "@/app/thunks/reservationsThunks";
import { UserType } from "@/types/user.types";
import { formatDateToServer } from "@/utils/dateTime";
import { FormEvent, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { toast } from "react-toastify";

export const useWaiterReservationDialog = (props: Props) => {
  const dispatch = useAppDispatch();
  const selectOptions = useSelector(selectSelectOptions);
  const selectOptionsLoading = useSelector(selectSelectOptionsLoading);
  const reservationCreatingLoading = useSelector(
    selectReservationCreatingLoading,
  );
  const waiter = useSelector(selectUser);
  const [userType, setUserType] = useState(UserType.Visitor);
  const allCustomers = useSelector(selectAllUsers);
  const allCustomersLoading = useSelector(selectAllUsersLoading);
  const [customerId, setCustomerId] = useState("");
  const maxGuests = 10;
  const [guests, setGuests] = useState(2);
  const [time, setTime] = useState("");
  const [date, setDate] = useState(props.initDate);
  const [table, setTable] = useState(props.initTable);
  const selectedCustomer = allCustomers?.find((c) => c.id === customerId);
  const customerName =
    `${selectedCustomer?.firstName ?? ""} ${selectedCustomer?.lastName ?? ""}`.trim();
  const locationTables = useSelector(selectLocationTables);

  useEffect(() => {
    setTable(props.initTable);
    setDate(props.initDate);
  }, [props.initTable, props.initDate]);

  const increaseGuests = () => {
    setGuests(guests < maxGuests ? guests + 1 : maxGuests);
  };

  const decreaseGuests = () => {
    setGuests(guests > 1 ? guests - 1 : 1);
  };

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!waiter?.locationId) {
      toast.error("The Waiter should have id of Location'");
    }
    if (userType === UserType.Customer && !customerId) {
      toast.error("Select 'Customer'");
    }
    if (!time) {
      toast.error("Select 'Time'");
    }
    if (!table) {
      toast.error("Select 'Table'");
    }

    try {
      await dispatch(
        upsertWaiterReservation({
          clientType: userType,
          date: formatDateToServer(date),
          guestsNumber: String(guests),
          locationId: waiter?.locationId ?? "",
          tableNumber:
            locationTables.find((t) => t.tableId === table)?.tableNumber ?? "",
          tableId: table,
          timeFrom: time.split(" - ")[0],
          timeTo: time.split(" - ")[1],
          customerName: customerName ?? "",
          customerId: selectedCustomer?.id ?? "",
        }),
      ).unwrap();
      props.onSuccessCallback();
      toast.success("New Reservation has been created successfully.");
    } catch (error) {
      if (error instanceof Error) {
        console.error("Reservation creating failed:", error);
        toast.error(
          `Reservation creating failed: ${"message" in error ? error.message : ""}`,
        );
      }
    }
  };

  return {
    selectOptions,
    selectOptionsLoading,
    userType,
    setUserType,
    waiter,
    allCustomers,
    customerId,
    setCustomerId,
    allCustomersLoading,
    guests,
    increaseGuests,
    decreaseGuests,
    time,
    setTime,
    table,
    setTable,
    onSubmit,
    date,
    setDate,
    reservationCreatingLoading,
  };
};

interface Props {
  initTable: string;
  initDate: string;
  onSuccessCallback: () => void;
}
