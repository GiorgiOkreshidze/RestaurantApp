import { Dish, GlobalErrorMessage, Location } from "@/types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

export const getLocations = createAsyncThunk<
  Location[],
  void,
  { rejectValue: GlobalErrorMessage }
>("locations/getLocations", async (_, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(serverRoute.locations);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

export const getSpecialityDishes = createAsyncThunk<
  Dish[],
  string,
  { rejectValue: GlobalErrorMessage }
>("locations/getSpecialityDishes", async (id, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(
      `${serverRoute.locations}/${id}/${serverRoute.specialityDishes}`,
    );
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
