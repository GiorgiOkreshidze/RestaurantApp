import { createSlice } from "@reduxjs/toolkit";
import type { TableUI } from "../../types/tables.types";
import { fetchTables } from "../thunks/tablesThunk";
import {
  dateStringServerToDateObject,
  timeString24hToDateObj,
} from "@/utils/dateTime";
import { isPast } from "date-fns";

interface TablesState {
  tables: TableUI[];
  tablesLoading: boolean;
}

const initialState: TablesState = {
  tables: [],
  tablesLoading: false,
};

export const tablesSlice = createSlice({
  name: "tables",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchTables.pending, (state) => {
        state.tablesLoading = true;
      })
      .addCase(fetchTables.fulfilled, (state, { payload }) => {
        state.tablesLoading = false;
        state.tables = payload.data
          .map((table) => ({
            ...table,
            date: dateStringServerToDateObject(payload.date),
            availableSlots: table.availableSlots
              .map((timeSlot) => ({
                id: `${timeSlot.start}-${timeSlot.end}`,
                startString: timeSlot.start,
                endString: timeSlot.end,
                rangeString: `${timeSlot.start}-${timeSlot.end}`,
                startDate: timeString24hToDateObj(timeSlot.start),
                endDate: timeString24hToDateObj(timeSlot.end),
                isPast: isPast(timeString24hToDateObj(timeSlot.start)),
              }))
              .sort((a, b) => (a.startDate > b.startDate ? 1 : -1)),
          }))
          .sort(
            (a, b) =>
              Number.parseInt(a.tableNumber) - Number.parseInt(b.tableNumber),
          );
      })
      .addCase(fetchTables.rejected, (state) => {
        state.tablesLoading = false;
      });
  },
  selectors: {
    selectTables: (state) => state.tables,
    selectTablesLoading: (state) => state.tablesLoading,
  },
});

export const tablesReducer = tablesSlice.reducer;
export const { selectTables, selectTablesLoading } = tablesSlice.selectors;
