import type { Dish, LocationTable, Review } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import {
  getFeedbacksOfLocation,
  getLocations,
  getLocationTables,
  getOneLocation,
  getSelectOptions,
  getSpecialityDishes,
} from "../thunks/locationsThunks";
import type { Location, SelectOption } from "@/types/location.types";

interface locationsState {
  locations: Location[];
  oneLocation: Location | null;
  specialityDishes: Dish[];
  feedbacks: Review[];
  totalPages: number;
  locationsLoading: boolean;
  selectOptions: SelectOption[];
  selectOptionsLoading: boolean;
  feedbacksLoading: boolean;
  locationTables: LocationTable[];
  locationTablesLoading: boolean;
}

const initialState: locationsState = {
  locations: [],
  oneLocation: null,
  specialityDishes: [],
  feedbacks: [],
  totalPages: 0,
  locationsLoading: false,
  selectOptions: [],
  selectOptionsLoading: false,
  feedbacksLoading: false,
  locationTables: [],
  locationTablesLoading: false,
};

export const locationsSlice = createSlice({
  name: "locations",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getLocations.pending, (state) => {
        state.locationsLoading = true;
      })
      .addCase(getLocations.fulfilled, (state, { payload: data }) => {
        state.locationsLoading = false;
        state.locations = data;
      })
      .addCase(getLocations.rejected, (state) => {
        state.locationsLoading = false;
      });

    builder
      .addCase(getOneLocation.pending, (state) => {
        state.locationsLoading = true;
      })
      .addCase(getOneLocation.fulfilled, (state, { payload: data }) => {
        state.locationsLoading = false;
        state.oneLocation = data;
      })
      .addCase(getOneLocation.rejected, (state) => {
        state.locationsLoading = false;
      });

    builder
      .addCase(getSpecialityDishes.pending, (state) => {
        state.locationsLoading = true;
      })
      .addCase(getSpecialityDishes.fulfilled, (state, { payload: data }) => {
        state.locationsLoading = false;
        state.specialityDishes = data;
      })
      .addCase(getSpecialityDishes.rejected, (state) => {
        state.locationsLoading = false;
      });
    builder
      .addCase(getFeedbacksOfLocation.pending, (state) => {
        state.feedbacksLoading = true;
      })
      .addCase(getFeedbacksOfLocation.fulfilled, (state, { payload: data }) => {
        state.feedbacksLoading = false;
        state.feedbacks = data.content;
      })
      .addCase(getFeedbacksOfLocation.rejected, (state) => {
        state.feedbacksLoading = false;
      });

    builder
      .addCase(getSelectOptions.pending, (state) => {
        state.selectOptionsLoading = true;
      })
      .addCase(getSelectOptions.fulfilled, (state, { payload: data }) => {
        state.selectOptionsLoading = false;
        state.selectOptions = data;
      })
      .addCase(getSelectOptions.rejected, (state) => {
        state.selectOptionsLoading = false;
      });
    builder
      .addCase(getLocationTables.pending, (state) => {
        state.locationTablesLoading = true;
      })
      .addCase(getLocationTables.fulfilled, (state, { payload: data }) => {
        state.locationTablesLoading = false;
        state.locationTables = data;
      })
      .addCase(getLocationTables.rejected, (state) => {
        state.locationTablesLoading = false;
      });
  },
  selectors: {
    selectLocations: (state) => state.locations,
    selectOneLocation: (state) => state.oneLocation,
    selectSpecialityDishes: (state) => state.specialityDishes,
    selectFeedbacks: (state) => state.feedbacks,
    selectTotalPages: (state) => state.totalPages,
    selectLocationsLoading: (state) => state.locationsLoading,
    selectSelectOptions: (state) => state.selectOptions,
    selectSelectOptionsLoading: (state) => state.selectOptionsLoading,
    selectFeedbacksLoading: (state) => state.feedbacksLoading,
    selectLocationTables: (state) => state.locationTables,
  },
});

export const locationsReducer = locationsSlice.reducer;
export const {
  selectLocations,
  selectOneLocation,
  selectSpecialityDishes,
  selectFeedbacks,
  selectTotalPages,
  selectLocationsLoading,
  selectSelectOptions,
  selectSelectOptionsLoading,
  selectFeedbacksLoading,
  selectLocationTables,
} = locationsSlice.selectors;
