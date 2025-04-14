import type {
  Dish,
  FeedbacksResponse,
  GlobalErrorMessage,
  LocationTable,
} from "@/types";
import type { Location, SelectOption } from "@/types/location.types";
import axiosApi, { axiosLOCAL } from "@/utils/axiosApi";
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

export const getOneLocation = createAsyncThunk<
  Location,
  string,
  { rejectValue: GlobalErrorMessage }
>("locations/getOneLocation", async (id, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(`${serverRoute.location}/${id}`);
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

export const getFeedbacksOfLocation = createAsyncThunk<
  FeedbacksResponse,
  { id: string; type: "SERVICE_QUALITY" | "CUISINE_EXPERIENCE"; sort: string },
  { rejectValue: GlobalErrorMessage }
>(
  "locations/getAllFeedbacksOfLocation",
  async (params, { rejectWithValue }) => {
    try {
      const response = await axiosApi.get(
        `${serverRoute.locations}/${params.id}/${serverRoute.feedbacks}`,
        { params: { type: params.type, sort: params.sort, size: "100" } },
      );
      return response.data;
    } catch (e) {
      if (isAxiosError(e) && e.response) {
        return rejectWithValue(e.response.data);
      }
      throw e;
    }
  },
);

export const getSelectOptions = createAsyncThunk<
  SelectOption[],
  void,
  { rejectValue: GlobalErrorMessage }
>("locations/select-options", async (_, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(serverRoute.selectOptions);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

export const getLocationTables = createAsyncThunk<
  LocationTable[],
  void,
  { rejectValue: GlobalErrorMessage }
>("location-tables", async (_, { rejectWithValue }) => {
  try {
    const response = await axiosLOCAL.get(serverRoute.locationTables);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
