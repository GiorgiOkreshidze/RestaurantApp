import { createSlice } from "@reduxjs/toolkit";
import { RichTimeSlot } from "@/types";
import { TIME_SLOTS } from "@/utils/constants";
import { startOfTomorrow } from "date-fns";

interface BookingFormState {
  locationId: string;
  date: Date;
  time: string;
  guests: number;
  timeSlots: RichTimeSlot[];
}

const initialState: BookingFormState = {
  locationId: "",
  date: TIME_SLOTS.filter((slot) => !slot.isPast).length
    ? new Date()
    : startOfTomorrow(),
  time: "",
  guests: 2,
  timeSlots: TIME_SLOTS,
};

export const bookingFormSlice = createSlice({
  name: "bookingForm",
  initialState,
  reducers: {
    setLocationIdAction: (state, { payload: data }) => {
      state.locationId = data;
    },

    setDateAction: (state, { payload: data }) => {
      state.date = data;
    },

    setTimeAction: (state, { payload: data }) => {
      state.time = data;
    },

    increaseGuestsAction: (state) => {
      state.guests = state.guests < 10 ? state.guests + 1 : 10;
    },

    decreaseGuestsAction: (state) => {
      state.guests = state.guests > 1 ? state.guests - 1 : 1;
    },
  },
  extraReducers: () => {},
  selectors: {
    selectBookingFormState: (state) => state,
  },
});

export const bookingFormReducer = bookingFormSlice.reducer;
export const {
  setLocationIdAction,
  setDateAction,
  setTimeAction,
  increaseGuestsAction,
  decreaseGuestsAction,
} = bookingFormSlice.actions;
export const { selectBookingFormState } = bookingFormSlice.selectors;
