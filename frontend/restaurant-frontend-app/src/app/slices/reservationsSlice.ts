import { Reservation } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import { getReservations } from "../thunks/reservationsThunks";

interface reservationsState {
  reservations: Reservation[];
  reservationsLoading: boolean;
}

const initialState: reservationsState = {
  reservations: [],
  reservationsLoading: false,
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
  },
  selectors: {
    selectReservations: (state) => state.reservations,
    selectReservationsLoading: (state) => state.reservationsLoading,
  },
});

export const reservationsReducer = reservationsSlice.reducer;
export const { selectReservations, selectReservationsLoading } =
  reservationsSlice.selectors;
