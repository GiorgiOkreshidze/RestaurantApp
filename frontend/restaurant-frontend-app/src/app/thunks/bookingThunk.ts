import {
  Table,
  GlobalErrorMessage,
  TableRequestParams,
  Reservation,
  UpsertReservationRequest,
} from "@/types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

export const getTables = createAsyncThunk<
  Table[],
  TableRequestParams,
  { rejectValue: GlobalErrorMessage }
>("bookings/tables", async (params, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(serverRoute.tables, {
      params: {
        locationId: params.locationId,
        date: params.date,
        time: params.time,
        guests: params.guests,
      },
    });
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

export const getLocationTimeSlots = createAsyncThunk<
  Table[],
  TableRequestParams,
  { rejectValue: GlobalErrorMessage }
>(
  "bookings/tables - locationTimeSlots",
  async (params, { rejectWithValue }) => {
    try {
      const response = await axiosApi.get(serverRoute.tables, {
        params: {
          locationId: params.locationId,
          date: params.date,
        },
      });
      return response.data;
    } catch (e) {
      if (isAxiosError(e) && e.response) {
        return rejectWithValue(e.response.data);
      }
      throw e;
    }
  },
);

export const upsertClientReservation = createAsyncThunk<
  Reservation,
  UpsertReservationRequest,
  { rejectValue: GlobalErrorMessage }
>("reservations/client", async (reservationMutation, { rejectWithValue }) => {
  try {
    const response = await axiosApi.post<Reservation>(
      serverRoute.upsertClientReservation,
      reservationMutation,
    );
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

export const deleteClientReservation = createAsyncThunk<
  void,
  string,
  { rejectValue: GlobalErrorMessage }
>("reservations/client/{id}", async (id, { rejectWithValue }) => {
  try {
    await axiosApi.delete(`${serverRoute.upsertClientReservation}/${id}`);
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
