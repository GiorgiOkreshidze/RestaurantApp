import type { Dish, GlobalErrorMessage } from "@/types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

export const getPopularDishes = createAsyncThunk<
  Dish[],
  void,
  { rejectValue: GlobalErrorMessage }
>("dishes/getPopularDishes", async (_, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(serverRoute.popularDishes);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
