import { useAppDispatch } from "@/app/hooks";

export const useWaiterReservationCreate = () => {
  const [guests, setGuests] = useState(
    reservation?.guestsNumber
      ? Number.parseInt(reservation.guestsNumber)
      : props.initGuests
  );

  const dispatch = useAppDispatch();

  const increaseGuests = () => {
    setGuests(guests < maxGuests ? guests + 1 : maxGuests);
  };

  const decreaseGuests = () => {
    setGuests(guests > 1 ? guests - 1 : 1);
  };
};
