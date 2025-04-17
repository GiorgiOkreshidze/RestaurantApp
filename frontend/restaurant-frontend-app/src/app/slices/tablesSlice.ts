import { createSlice } from "@reduxjs/toolkit";
import type { TableUI } from "../../types/tables.types";
import { getTables } from "../thunks/tablesThunk";
import {
  parseDateFromServer,
  parseTimeFromServer,
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
      .addCase(getTables.pending, (state) => {
        state.tablesLoading = true;
      })
      .addCase(getTables.fulfilled, (state, { payload }) => {
        state.tablesLoading = false;
        state.tables = payload.data
          .map((table) => ({
            ...table,
            date: parseDateFromServer(payload.date),
            availableSlots: table.availableSlots
              .map((timeSlot) => ({
                id: `${timeSlot.start} - ${timeSlot.end}`,
                startString: timeSlot.start,
                endString: timeSlot.end,
                rangeString: `${timeSlot.start} - ${timeSlot.end}`,
                startDate: parseTimeFromServer(timeSlot.start),
                endDate: parseTimeFromServer(timeSlot.end),
                isPast: isPast(parseTimeFromServer(timeSlot.start)),
              }))
              .sort((a, b) => (a.startDate > b.startDate ? 1 : -1)),
          }))
          .sort(
            (a, b) =>
              Number.parseInt(a.tableNumber) - Number.parseInt(b.tableNumber),
          );
      })
      .addCase(getTables.rejected, (state) => {
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
