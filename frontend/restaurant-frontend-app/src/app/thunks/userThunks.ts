import {
  GlobalErrorMessage,
  RegisterMutation,
  RegisterResponse,
} from "@/types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

export const register = createAsyncThunk<
  RegisterResponse,
  RegisterMutation,
  { rejectValue: GlobalErrorMessage }
>("users/signup", async (registerMutation, { rejectWithValue }) => {
  try {
    const response = await axiosApi.post(serverRoute.signUp, registerMutation);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
