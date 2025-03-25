import { Table, GlobalErrorMessage, GetTablesParams } from "@/types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

export const getTables = createAsyncThunk<
  Table[],
  GetTablesParams,
  { rejectValue: GlobalErrorMessage }
>("booking/tables", async (params, { rejectWithValue }) => {
  try {
    const response = await axiosApi.get(serverRoute.tables, {
      params: {
        date: params.date,
        locationId: params.locationId,
        guests: params.guests,
        time: params.time,
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
