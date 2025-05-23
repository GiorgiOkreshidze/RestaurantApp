import type { Dish, ExtendedDish } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import {
  getAllDishes,
  getOneDish,
  getPopularDishes,
} from "../thunks/dishesThunks";

interface dishesState {
  dishes: Dish[];
  oneDish: ExtendedDish | null;
  popularDishes: Dish[];
  dishesLoading: boolean;
  oneDishLoading: boolean;
}

const initialState: dishesState = {
  dishes: [],
  oneDish: null,
  popularDishes: [],
  dishesLoading: false,
  oneDishLoading: false,
};

export const dishesSlice = createSlice({
  name: "dishes",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getAllDishes.pending, (state) => {
        state.dishesLoading = true;
      })
      .addCase(getAllDishes.fulfilled, (state, { payload: data }) => {
        state.dishesLoading = false;
        state.dishes = data;
      })
      .addCase(getAllDishes.rejected, (state) => {
        state.dishesLoading = false;
      });
    builder
      .addCase(getOneDish.pending, (state) => {
        state.oneDishLoading = true;
      })
      .addCase(getOneDish.fulfilled, (state, { payload: data }) => {
        state.oneDishLoading = false;
        state.oneDish = data;
      })
      .addCase(getOneDish.rejected, (state) => {
        state.oneDishLoading = false;
      });
    builder
      .addCase(getPopularDishes.pending, (state) => {
        state.dishesLoading = true;
      })
      .addCase(getPopularDishes.fulfilled, (state, { payload: data }) => {
        state.dishesLoading = false;
        state.popularDishes = data;
      })
      .addCase(getPopularDishes.rejected, (state) => {
        state.dishesLoading = false;
      });
  },
  selectors: {
    selectDishes: (state) => state.dishes,
    selectOneDish: (state) => state.oneDish,
    selectPopularDishes: (state) => state.popularDishes,
    selectDishesLoading: (state) => state.dishesLoading,
    selectOneDishLoading: (state) => state.oneDishLoading,
  },
});

export const dishesReducer = dishesSlice.reducer;
export const {
  selectDishes,
  selectOneDish,
  selectPopularDishes,
  selectDishesLoading,
  selectOneDishLoading,
} = dishesSlice.selectors;
