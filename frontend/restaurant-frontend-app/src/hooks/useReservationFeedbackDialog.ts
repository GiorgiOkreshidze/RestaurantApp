import { useAppDispatch } from "@/app/hooks";
import { selectGiveReservationFeedbackLoading } from "@/app/slices/reservationsSlice";
import {
  getReservations,
  giveReservationFeedback,
} from "@/app/thunks/reservationsThunks";
import { FormEvent, useState } from "react";
import { useSelector } from "react-redux";
import { toast } from "react-toastify";

export const useReservationFeedbackDialog = (props: Props) => {
  const dispatch = useAppDispatch();
  const [serviceRating, setServiceRating] = useState(4);
  const [serviceComment, setServiceComment] = useState("");
  const [culinaryRating, setCulinaryRating] = useState(4);
  const [culinaryComment, setCulinaryComment] = useState("");
  const giveReservationFeedbackLoading = useSelector(
    selectGiveReservationFeedbackLoading,
  );

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    try {
      const data = await dispatch(
        giveReservationFeedback({
          cuisineComment: culinaryComment,
          cuisineRating: String(culinaryRating),
          reservationId: props.reservationId,
          serviceComment: serviceComment,
          serviceRating: String(serviceRating),
        }),
      ).unwrap();
      props.onSuccessCallback();
      await dispatch(getReservations({}));
      console.log("Reservation created", data);
    } catch (error) {
      if (error instanceof Error) {
        console.error("Feedback creating failed:", error);
        toast.error(
          `Feedback creating failed: ${"message" in error ? error.message : ""}`,
        );
      }
    }
  };

  return {
    serviceRating,
    setServiceRating,
    serviceComment,
    setServiceComment,
    culinaryRating,
    setCulinaryRating,
    culinaryComment,
    setCulinaryComment,
    reservationId: props.reservationId,
    onSubmit,
    giveReservationFeedbackLoading,
  };
};

interface Props {
  reservationId: string;
  onSuccessCallback: () => void;
}
