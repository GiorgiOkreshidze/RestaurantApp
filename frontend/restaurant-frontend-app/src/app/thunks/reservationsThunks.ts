import { Reservation, GlobalErrorMessage } from "@/types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

export const getReservations = createAsyncThunk<
  Reservation[],
  void,
  { rejectValue: GlobalErrorMessage }
>("reservations", async (_, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(serverRoute.reservations);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
