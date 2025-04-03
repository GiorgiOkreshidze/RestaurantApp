import type { GlobalErrorMessage } from "@/types";
import type {
  GetReservationRequestParams,
  Reservation,
  UpsertReservationRequestParams,
  UpsertWaiterReservationRequestParams,
} from "@/types/reservation.types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

export const getReservations = createAsyncThunk<
  Reservation[],
  GetReservationRequestParams,
  { rejectValue: GlobalErrorMessage }
>("reservations", async (params, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(serverRoute.reservations, {
      params,
    });
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

export const upsertClientReservation = createAsyncThunk<
  Reservation,
  UpsertReservationRequestParams,
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
>("reservations/{id}", async (id, { rejectWithValue }) => {
  try {
    await axiosApi.delete(`${serverRoute.deleteClientReservation}/${id}`);
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});

export const upsertWaiterReservation = createAsyncThunk<
  Reservation,
  UpsertWaiterReservationRequestParams,
  { rejectValue: GlobalErrorMessage }
>(
  "api/reservations/waiter",
  async (reservationMutation, { rejectWithValue }) => {
    try {
      const response = await axiosApi.post<Reservation>(
        serverRoute.upsertWaiterReservation,
        reservationMutation,
      );
      return response.data;
    } catch (e) {
      if (isAxiosError(e)) {
        return e.response
          ? rejectWithValue(e.response.data)
          : rejectWithValue({ message: e.message });
      }
      throw e;
    }
  },
);
