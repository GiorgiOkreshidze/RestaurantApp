import type { Dish, ExtendedDish, GlobalErrorMessage } from "@/types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

interface GetAllDishesParams {
  category?: string;
  sortBy?: string;
}

export const getAllDishes = createAsyncThunk<
  Dish[],
  GetAllDishesParams,
  { rejectValue: GlobalErrorMessage }
>("dishes/getAll", async (params, { rejectWithValue }) => {
  try {
    const { category, sortBy } = params;
    let url = serverRoute.dishes;
    const queryParams = new URLSearchParams();

    if (category) {
      queryParams.append("dishType", category);
    }

    if (sortBy) {
      queryParams.append("sort", sortBy);
    }

    const queryString = queryParams.toString();
    if (queryString) {
      url += `?${queryString}`;
    }

    const response = await axiosApi.get(url);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

export const getOneDish = createAsyncThunk<
  ExtendedDish,
  string,
  { rejectValue: GlobalErrorMessage }
>("dishes/getOne", async (id, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(`${serverRoute.dishes}/${id}`);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

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
