import { createSlice } from "@reduxjs/toolkit";
import { LocationTable, RichTimeSlot } from "@/types";
import { LOCATION_TABLES, TIME_SLOTS } from "@/utils/constants";

interface WaiterReservationsState {
  form: WaiterReservationsFormState;
}

export interface WaiterReservationsFormState {
  date: string;
  time: string;
  table: string;
  timeList: RichTimeSlot[];
  tableList: LocationTable[];
}

const initialState: WaiterReservationsState = {
  form: {
    date: new Date().toString(),
    time: "",
    table: "",
    timeList: TIME_SLOTS,
    tableList: LOCATION_TABLES,
  },
};

export const waiterReservationsState = createSlice({
  name: "waiterReservations",
  initialState,
  reducers: {
    setFormDateAction: (state, { payload: data }: { payload: string }) => {
      state.form.date = data;
    },
    setFormTimeAction: (state, { payload: data }: { payload: string }) => {
      state.form.time = data;
    },
    setFormTableAction: (state, { payload: data }: { payload: string }) => {
      state.form.table = data;
    },
  },
  extraReducers: () => {},
  selectors: {
    selectWaiterReservationsForm: (state) => state.form,
  },
});

export const waiterReservationsReducer = waiterReservationsState.reducer;
export const { setFormDateAction, setFormTimeAction, setFormTableAction } =
  waiterReservationsState.actions;
export const { selectWaiterReservationsForm } =
  waiterReservationsState.selectors;
