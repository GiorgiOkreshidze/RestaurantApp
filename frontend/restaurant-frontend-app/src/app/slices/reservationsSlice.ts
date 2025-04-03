import { createSlice } from "@reduxjs/toolkit";
import {
  deleteClientReservation,
  getReservations,
  upsertClientReservation,
  upsertWaiterReservation,
} from "../thunks/reservationsThunks";
import type { Reservation } from "@/types/reservation.types";
import { toast } from "react-toastify";

interface reservationsState {
  reservations: Reservation[];
  reservationsLoading: boolean;
  reservation: Reservation | null;
  reservationCreatingLoading: boolean;
  reservationDeletingLoading: boolean;
}

const initialState: reservationsState = {
  reservations: [],
  reservationsLoading: false,
  reservation: null,
  reservationCreatingLoading: false,
  reservationDeletingLoading: false,
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
      .addCase(getReservations.fulfilled, (state, { payload: data }) => {
        state.reservationsLoading = false;
        state.reservations = data;
      })
      .addCase(getReservations.rejected, (state) => {
        state.reservationsLoading = false;
      });

    builder
      .addCase(upsertClientReservation.pending, (state) => {
        state.reservationCreatingLoading = true;
      })
      .addCase(
        upsertClientReservation.fulfilled,
        (state, { payload: data }) => {
          state.reservationCreatingLoading = false;
          state.reservation = data;
        },
      )
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
      .addCase(
        upsertWaiterReservation.fulfilled,
        (state, { payload: data }) => {
          state.reservationCreatingLoading = false;
          state.reservation = data;
        },
      )
      .addCase(
        upsertWaiterReservation.rejected,
        (state, { payload: errorResponse }) => {
          state.reservationCreatingLoading = false;
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
  },
});

export const reservationsReducer = reservationsSlice.reducer;
export const {
  selectReservations,
  selectReservationsLoading,
  selectReservationCreatingLoading,
  selectReservationDeletingLoading,
} = reservationsSlice.selectors;
