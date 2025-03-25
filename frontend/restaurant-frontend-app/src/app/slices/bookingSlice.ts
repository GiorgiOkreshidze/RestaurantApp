import { BookingFilters, Table } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import { getTables } from "../thunks/bookingThunk";

interface BookingState {
  tables: Table[];
  tablesLoading: boolean;
  filters: BookingFilters;
}

const initialState: BookingState = {
  tables: [],
  tablesLoading: false,
  filters: {
    locationId: null,
    dateTime: new Date().toString(),
    guestsNumber: 2,
  },
};

export const bookingSlice = createSlice({
  name: "booking",
  initialState,
  reducers: {
    setFilters: (state, { payload: filters }) => {
      state.filters = filters;
      // console.log(state.filters.dateTime);
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(getTables.pending, (state) => {
        state.tablesLoading = true;
      })
      .addCase(getTables.fulfilled, (state, { payload: data }) => {
        state.tables = data;
        state.tablesLoading = false;
      })
      .addCase(getTables.rejected, (state) => {
        state.tablesLoading = false;
      });
  },
  selectors: {
    selectTables: (state) => state.tables,
    selectTablesLoading: (state) => state.tablesLoading,
    selectFilters: (state) => state.filters,
  },
});

export const bookingReducer = bookingSlice.reducer;
export const { setFilters } = bookingSlice.actions;
export const { selectTables, selectTablesLoading, selectFilters } =
  bookingSlice.selectors;
