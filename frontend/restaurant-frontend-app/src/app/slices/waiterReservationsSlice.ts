import { createSlice } from "@reduxjs/toolkit";
import { LocationTable, RichTimeSlot } from "@/types";
import { LOCATION_TABLES } from "@/utils/constants";
import { startOfToday } from "date-fns";
import { getTimeSlots } from "../thunks/bookingThunk";

const initialState: WaiterReservationsState = {
  form: {
    date: startOfToday().toString(),
    time: "",
    table: "",
    timeList: [],
    tableList: LOCATION_TABLES,
  },
  newReservation: null,
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
  extraReducers: (builder) => {
    builder.addCase(getTimeSlots.fulfilled, (state, { payload: data }) => {
      state.form.timeList = data;
    });
  },
  selectors: {
    selectWaiterReservationsForm: (state) => state.form,
  },
});

export const waiterReservationsReducer = waiterReservationsState.reducer;
export const { setFormDateAction, setFormTimeAction, setFormTableAction } =
  waiterReservationsState.actions;
export const { selectWaiterReservationsForm } =
  waiterReservationsState.selectors;

export interface WaiterReservationsState {
  form: WaiterReservationsSearchForm;
  newReservation: NewReservation | null;
}
export interface WaiterReservationsSearchForm {
  date: string;
  time: string;
  table: string;
  timeList: RichTimeSlot[];
  tableList: LocationTable[];
}

export interface NewReservation {
  locationId: string;
}
