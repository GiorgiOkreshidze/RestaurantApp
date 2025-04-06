import type { GlobalErrorMessage } from "@/types";
import type { TableServer, TablesRequestParams } from "@/types/tables.types";
import axiosApi from "@/utils/axiosApi";
import { serverRoute } from "@/utils/constants";
import { createAsyncThunk } from "@reduxjs/toolkit";
import { isAxiosError } from "axios";

export const getTables = createAsyncThunk<
  { data: TableServer[]; date: string },
  TablesRequestParams,
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
    return { data: response.data, date: params.date };
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
