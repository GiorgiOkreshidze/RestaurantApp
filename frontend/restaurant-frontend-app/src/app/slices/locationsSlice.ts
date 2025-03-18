import { Dish, Location } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import { getLocations, getSpecialityDishes } from "../thunks/locationsThunks";

interface locationsState {
  locations: Location[];
  oneLocation: Location | null;
  specialityDishes: Dish[];
  locationsLoading: boolean;
}

const initialState: locationsState = {
  locations: [],
  oneLocation: null,
  specialityDishes: [],
  locationsLoading: false,
};

export const locationsSlice = createSlice({
  name: "locations",
  initialState,
  reducers: {
    setOneLocation: (state, { payload: data }) => {
      state.oneLocation = data;
    },
  },
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
  },
  selectors: {
    selectLocations: (state) => state.locations,
    selectOneLocation: (state) => state.oneLocation,
    selectSpecialityDishes: (state) => state.specialityDishes,
    selectLocationsLoading: (state) => state.locationsLoading,
  },
});

export const locationsReducer = locationsSlice.reducer;
export const { setOneLocation } = locationsSlice.actions;
export const {
  selectLocations,
  selectOneLocation,
  selectSpecialityDishes,
  selectLocationsLoading,
} = locationsSlice.selectors;
