import { Reservation, Table } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import {
  upsertClientReservation,
  getLocationTimeSlots,
  getTables,
  deleteClientReservation,
} from "../thunks/bookingThunk";
import { getHours, parse } from "date-fns";

interface BookingState {
  tables: Table[];
  tablesLoading: boolean;
  locationTimeSlots: string[];
  todaySlots: string[];
  locationTimeSlotsLoading: boolean;
  reservation: Reservation | null;
  reservationCreatingLoading: boolean;
  reservationDeletingLoading: boolean;
}

const initialState: BookingState = {
  tables: [],
  tablesLoading: false,
  locationTimeSlots: [],
  locationTimeSlotsLoading: false,
  todaySlots: [],
  reservation: null,
  reservationCreatingLoading: false,
  reservationDeletingLoading: false,
};

export const bookingSlice = createSlice({
  name: "booking",
  initialState,
  reducers: {
    clearLocationTimeSlots: (state) => {
      state.locationTimeSlots = [];
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(getTables.pending, (state) => {
        state.tablesLoading = true;
      })
      .addCase(getTables.fulfilled, (state, { payload: data }) => {
        state.tablesLoading = false;
        state.tables = data;
      })
      .addCase(getTables.rejected, (state) => {
        state.tablesLoading = false;
      });

    builder
      .addCase(getLocationTimeSlots.pending, (state) => {
        state.locationTimeSlotsLoading = true;
      })
      .addCase(getLocationTimeSlots.fulfilled, (state, { payload: data }) => {
        state.locationTimeSlotsLoading = false;
        const slots = data
          .flatMap((slot) => slot.availableSlots)
          .map((slot) => `${slot.start}-${slot.end}`)
          .sort((a, b) => {
            const fromA = parse(a.split("-")[0], "HH:mm", new Date());
            const fromB = parse(b.split("-")[0], "HH:mm", new Date());
            return fromA > fromB ? 1 : -1;
          });
        const uniqueSlots = [...new Set(slots)];
        state.locationTimeSlots = uniqueSlots;
        state.todaySlots = slots.filter((slot) => {
          const from = parse(slot.split("-")[0], "HH:mm", new Date());
          return from > new Date();
        });
      })
      .addCase(getLocationTimeSlots.rejected, (state) => {
        state.locationTimeSlotsLoading = false;
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
      .addCase(upsertClientReservation.rejected, (state) => {
        state.reservationCreatingLoading = false;
      });

    builder
      .addCase(deleteClientReservation.pending, (state) => {
        state.reservationDeletingLoading = true;
      })
      .addCase(
        deleteClientReservation.fulfilled,
        (state, { payload: data }) => {
          state.reservationDeletingLoading = false;
        },
      )
      .addCase(deleteClientReservation.rejected, (state) => {
        state.reservationDeletingLoading = false;
      });
  },
  selectors: {
    selectTables: (state) => state.tables,
    selectTablesLoading: (state) => state.tablesLoading,
    selectLocationTimeSlots: (state) => state.locationTimeSlots,
    selectLocationTimeSlotsLoading: (state) => state.locationTimeSlotsLoading,
    selectTodaySlots: (state) => state.todaySlots,
    selectReservation: (state) => state.reservation,
    selectReservationCreatingLoading: (state) =>
      state.reservationCreatingLoading,
    selectReservationDeletingLoading: (state) =>
      state.reservationDeletingLoading,
  },
});

export const { clearLocationTimeSlots } = bookingSlice.actions;
export const bookingReducer = bookingSlice.reducer;
export const {
  selectTables,
  selectTablesLoading,
  selectLocationTimeSlots,
  selectLocationTimeSlotsLoading,
  selectReservation,
  selectReservationCreatingLoading,
  selectTodaySlots,
  selectReservationDeletingLoading,
} = bookingSlice.selectors;
