import { useAppDispatch } from "@/app/hooks";
import {
  selectSelectOptions,
  selectSelectOptionsLoading,
} from "@/app/slices/locationsSlice";
import {
  selectUser,
  selectAllUsers,
  selectAllUsersLoading,
} from "@/app/slices/userSlice";
import { upsertWaiterReservation } from "@/app/thunks/reservationsThunks";
import { UserType } from "@/types/user.types";
import { LOCATION_TABLES } from "@/utils/constants";
import { dateObjToDateStringServer } from "@/utils/dateTime";
import { FormEvent, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { toast } from "react-toastify";

export const useWaiterReservationDialog = (props: Props) => {
  const dispatch = useAppDispatch();
  const selectOptions = useSelector(selectSelectOptions);
  const selectOptionsLoading = useSelector(selectSelectOptionsLoading);
  const waiter = useSelector(selectUser);
  const [userType, setUserType] = useState(UserType.VISITOR);
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
    if (userType === UserType.CUSTOMER && !customerId) {
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
          date: dateObjToDateStringServer(date),
          guestsNumber: String(guests),
          locationId: waiter?.locationId ?? "",
          tableNumber:
            LOCATION_TABLES.find((t) => t.tableId === table)?.tableNumber ?? "",
          tableId: table,
          timeFrom: time.split(" - ")[0],
          timeTo: time.split(" - ")[1],
          customerName: customerName ?? "",
        }),
      );
    } catch (e) {
      console.log(e);
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
  };
};

interface Props {
  initTable: string;
  initDate: string;
}
