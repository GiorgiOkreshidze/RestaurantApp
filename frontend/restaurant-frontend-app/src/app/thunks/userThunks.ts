import { RegisterMutation, RegisterResponse } from "@/types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";

export const register = createAsyncThunk<RegisterResponse, RegisterMutation>(
  "users/register",
  async (registerMutation) => {
    try {
      const response = await axiosApi.post(serverRoute.users, registerMutation);
      return response.data;
    } catch (e) {
      console.error(e);
    }
  }
);
