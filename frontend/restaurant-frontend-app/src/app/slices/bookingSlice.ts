import { createSlice } from "@reduxjs/toolkit";
import { RichTimeSlot } from "@/types";
import { startOfTomorrow } from "date-fns";
import { getTimeSlots } from "../thunks/bookingThunk";

interface BookingState {
  locationId: string;
  date: string;
  time: string;
  guests: number;
  timeSlots: RichTimeSlot[];
  timeSlotsLoading: boolean;
}

export const initialState: BookingState = {
  locationId: "",
  date: new Date().toString(),
  time: "",
  guests: 2,
  timeSlots: [],
  timeSlotsLoading: false,
};

export const bookingSlice = createSlice({
  name: "booking",
  initialState,
  reducers: {
    setLocationAction: (state, { payload: data }) => {
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
  extraReducers: (builder) => {
    builder
      .addCase(getTimeSlots.pending, (state) => {
        state.timeSlotsLoading = true;
      })
      .addCase(getTimeSlots.fulfilled, (state, { payload: timeSlots }) => {
        state.timeSlotsLoading = false;
        state.timeSlots = timeSlots;
        state.date = timeSlots.filter((slot) => !slot.isPast).length
          ? new Date().toString()
          : startOfTomorrow().toString();
        state.time =
          timeSlots.find((slot) => !slot.isPast)?.rangeString ??
          timeSlots[0].rangeString;
      })
      .addCase(getTimeSlots.rejected, (state) => {
        state.timeSlotsLoading = false;
      });
  },
  selectors: {
    selectBooking: (state) => state,
  },
});

export const bookingReducer = bookingSlice.reducer;
export const {
  setLocationAction,
  setDateAction,
  setTimeAction,
  increaseGuestsAction,
  decreaseGuestsAction,
} = bookingSlice.actions;

export const { selectBooking } = bookingSlice.selectors;
