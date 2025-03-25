import { Table } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import { getTables } from "../thunks/tablesThunk";

interface tablesState {
  tables: Table[];
  availableTimes: Table["availableTimes"];
  tablesLoading: boolean;
}

const initialState: tablesState = {
  tables: [],
  availableTimes: [],
  tablesLoading: false,
};

export const tablesSlice = createSlice({
  name: "tables",
  initialState,
  reducers: {
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
        state.tablesLoading = false;
        state.tables = data;
        // state.availableTimes = data.map(())
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
