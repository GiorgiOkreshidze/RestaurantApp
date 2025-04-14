import { createSlice } from "@reduxjs/toolkit";
import {
  deleteClientReservation,
  getReservations,
  giveReservationFeedback,
  upsertClientReservation,
  upsertWaiterReservation,
} from "../thunks/reservationsThunks";
import type { Reservation } from "@/types/reservation.types";
import { toast } from "react-toastify";

interface reservationsState {
  reservations: Reservation[];
  reservationsLoading: boolean;
  reservationCreatingLoading: boolean;
  reservationDeletingLoading: boolean;
  giveReservationFeedbackLoading: boolean;
}

const initialState: reservationsState = {
  reservations: [],
  reservationsLoading: false,
  reservationCreatingLoading: false,
  reservationDeletingLoading: false,
  giveReservationFeedbackLoading: false,
};

export const reservationsSlice = createSlice({
  name: "reservations",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getReservations.pending, (state) => {
        state.reservationsLoading = true;
      })
      .addCase(
        getReservations.fulfilled,
        (state, { payload }: { payload: Reservation[] }) => {
          state.reservationsLoading = false;
          state.reservations = payload.map((reservation) => ({
            ...reservation,
            preOrder: reservation.id,
            timeTo: reservation.timeSlot.split(" - ")[1]
          }));
        },
      )
      .addCase(getReservations.rejected, (state) => {
        state.reservationsLoading = false;
      });

    builder
      .addCase(upsertClientReservation.pending, (state) => {
        state.reservationCreatingLoading = true;
      })
      .addCase(upsertClientReservation.fulfilled, (state) => {
        state.reservationCreatingLoading = false;
      })
      .addCase(
        upsertClientReservation.rejected,
        (state, { payload: errorResponse }) => {
          state.reservationCreatingLoading = false;
          toast.error(errorResponse?.message);
        },
      );

    builder
      .addCase(deleteClientReservation.pending, (state) => {
        state.reservationDeletingLoading = true;
      })
      .addCase(deleteClientReservation.fulfilled, (state) => {
        state.reservationDeletingLoading = false;
      })
      .addCase(deleteClientReservation.rejected, (state) => {
        state.reservationDeletingLoading = false;
      });

    builder
      .addCase(upsertWaiterReservation.pending, (state) => {
        state.reservationCreatingLoading = true;
      })
      .addCase(upsertWaiterReservation.fulfilled, (state) => {
        state.reservationCreatingLoading = false;
      })
      .addCase(
        upsertWaiterReservation.rejected,
        (state, { payload: errorResponse }) => {
          state.reservationCreatingLoading = false;
          toast.error(errorResponse?.message);
        },
      );

    builder
      .addCase(giveReservationFeedback.pending, (state) => {
        state.giveReservationFeedbackLoading = true;
      })
      .addCase(giveReservationFeedback.fulfilled, (state) => {
        state.giveReservationFeedbackLoading = false;
      })
      .addCase(
        giveReservationFeedback.rejected,
        (state, { payload: errorResponse }) => {
          state.giveReservationFeedbackLoading = false;
          toast.error(errorResponse?.message);
        },
      );
  },
  selectors: {
    selectReservations: (state) => state.reservations,
    selectReservationsLoading: (state) => state.reservationsLoading,
    selectReservationCreatingLoading: (state) =>
      state.reservationCreatingLoading,
    selectReservationDeletingLoading: (state) =>
      state.reservationDeletingLoading,
    selectGiveReservationFeedbackLoading: (state) =>
      state.giveReservationFeedbackLoading,
  },
});

export const reservationsReducer = reservationsSlice.reducer;
export const {
  selectReservations,
  selectReservationsLoading,
  selectReservationCreatingLoading,
  selectReservationDeletingLoading,
  selectGiveReservationFeedbackLoading,
} = reservationsSlice.selectors;
