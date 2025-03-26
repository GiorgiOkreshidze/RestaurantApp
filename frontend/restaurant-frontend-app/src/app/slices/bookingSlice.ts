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
    locationId: "",
    dateTime: new Date().toString(),
    guests: 2,
  },
};

export const bookingSlice = createSlice({
  name: "booking",
  initialState,
  reducers: {
    setFilters: (state, { payload: filters }) => {
      state.filters = filters;
    },
    clearTables: (state) => {
      state.tables = [];
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(getTables.pending, (state) => {
        state.tablesLoading = true;
      })
      .addCase(getTables.fulfilled, (state, { payload: data }) => {
        const newData = data.sort(
          (a, b) =>
            Number.parseInt(a.tableNumber) - Number.parseInt(b.tableNumber),
        );
        state.tables = newData;
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
export const { setFilters, clearTables } = bookingSlice.actions;
export const { selectTables, selectTablesLoading, selectFilters } =
  bookingSlice.selectors;
