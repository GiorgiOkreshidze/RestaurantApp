import { GlobalErrorMessage, RichTimeSlot } from "@/types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

export const getTimeSlots = createAsyncThunk<
  RichTimeSlot[],
  void,
  { rejectValue: GlobalErrorMessage }
>("timeslots", async (_, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(serverRoute.timeSlots);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
