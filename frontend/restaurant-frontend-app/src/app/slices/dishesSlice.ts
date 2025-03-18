import { Dish } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import { getPopularDishes } from "../thunks/dishesThunks";

interface dishesState {
  dishes: Dish[];
  popularDishes: Dish[];
  dishesLoading: boolean;
}

const initialState: dishesState = {
  dishes: [],
  popularDishes: [],
  dishesLoading: false,
};

export const dishesSlice = createSlice({
  name: "dishes",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
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
    selectPopularDishes: (state) => state.popularDishes,
    selectDishesLoading: (state) => state.dishesLoading,
  },
});

export const dishesReducer = dishesSlice.reducer;
export const { selectDishes, selectPopularDishes, selectDishesLoading } =
  dishesSlice.selectors;
