import { AppDispatch } from "./../store";
import {
  GlobalErrorMessage,
  LoginMutation,
  LoginResponse,
  RegisterMutation,
  RegisterResponse,
  signOutMutation,
  UserDataResponse,
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

export const login = createAsyncThunk<
  LoginResponse,
  LoginMutation,
  { rejectValue: GlobalErrorMessage }
>("users/login", async (loginMutation, { rejectWithValue }) => {
  try {
    const response = await axiosApi.post<LoginResponse>(
      serverRoute.signIn,
      loginMutation
    );
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

export const getUserData = createAsyncThunk<
  UserDataResponse,
  void,
  { rejectValue: GlobalErrorMessage }
>("users/profile", async (_, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get<UserDataResponse>(serverRoute.userData);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

export const signout = createAsyncThunk<
  void,
  signOutMutation,
  { rejectValue: GlobalErrorMessage; dispatch: AppDispatch }
>("signout", async (signOutMutation, { rejectWithValue }) => {
  try {
    await axiosApi.post(serverRoute.signOut, signOutMutation);
    console.log("Logged out successfully");
    return;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
