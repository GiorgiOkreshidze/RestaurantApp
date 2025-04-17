import { describe, it, expect, vi, beforeEach } from "vitest";
import { configureStore, EnhancedStore } from "@reduxjs/toolkit";
import {
  dishesReducer,
  initialState,
  selectDishes,
  selectOneDish,
  selectPopularDishes,
  selectDishesLoading,
  selectOneDishLoading,
} from "../dishesSlice";
import {
  getAllDishes,
  getOneDish,
  getPopularDishes,
} from "@/app/thunks/dishesThunks";
import type { Dish, ExtendedDish } from "@/types";


interface RootState {
  dishes: typeof initialState;
}

describe("dishesSlice", () => {
  let store: EnhancedStore<RootState>;

  beforeEach(() => {
    vi.clearAllMocks();
    store = configureStore({
      reducer: {
        dishes: dishesReducer,
      },
    });
  });

  describe("initial state", () => {
    it("should have the correct initial state", () => {
      expect(store.getState().dishes).toEqual(initialState);
    });
  });

  describe("selectors", () => {
    it("should select dishes", () => {
      expect(selectDishes(store.getState())).toEqual([]);
    });

    it("should select oneDish", () => {
      expect(selectOneDish(store.getState())).toBe(null);
    });

    it("should select popularDishes", () => {
      expect(selectPopularDishes(store.getState())).toEqual([]);
    });

    it("should select dishesLoading", () => {
      expect(selectDishesLoading(store.getState())).toBe(false);
    });

    it("should select oneDishLoading", () => {
      expect(selectOneDishLoading(store.getState())).toBe(false);
    });
  });

  describe("extraReducers", () => {
    it("should handle getAllDishes.pending", () => {
      store.dispatch({ type: getAllDishes.pending.type });
      expect(store.getState().dishes.dishesLoading).toBe(true);
    });

    it("should handle getAllDishes.fulfilled", () => {
      const mockDishes: Dish[] = [
        {
          id: "1",
          name: "Pizza",
          price: "10",
          weight: "5kg",
          imageUrl: "example.com",
        },
        {
          id: "2",
          name: "Burger",
          price: "8",
          weight: "5kg",
          imageUrl: "example.com",
        },
      ];
      store.dispatch({
        type: getAllDishes.fulfilled.type,
        payload: mockDishes,
      });

      expect(store.getState().dishes.dishesLoading).toBe(false);
      expect(store.getState().dishes.dishes).toEqual(mockDishes);
    });

    it("should handle getAllDishes.rejected", () => {
      store.dispatch({ type: getAllDishes.rejected.type });
      expect(store.getState().dishes.dishesLoading).toBe(false);
    });

    it("should handle getOneDish.pending", () => {
      store.dispatch({ type: getOneDish.pending.type });
      expect(store.getState().dishes.oneDishLoading).toBe(true);
    });

    it("should handle getOneDish.fulfilled", () => {
      const mockDish: ExtendedDish = {
        id: "1",
        name: "Pasta",
        price: "12",
        description: "Delicious pasta with sauce",
        calories: "1",
        carbohydrates: "1",
        fats: "1",
        proteins: "1",
        vitamins: "1",
        weight: "1",
        imageUrl: "1",
      };
      store.dispatch({ type: getOneDish.fulfilled.type, payload: mockDish });

      expect(store.getState().dishes.oneDishLoading).toBe(false);
      expect(store.getState().dishes.oneDish).toEqual(mockDish);
    });

    it("should handle getOneDish.rejected", () => {
      store.dispatch({ type: getOneDish.rejected.type });
      expect(store.getState().dishes.oneDishLoading).toBe(false);
    });

    it("should handle getPopularDishes.pending", () => {
      store.dispatch({ type: getPopularDishes.pending.type });
      expect(store.getState().dishes.dishesLoading).toBe(true);
    });

    it("should handle getPopularDishes.fulfilled", () => {
      const mockPopularDishes: Dish[] = [
        {
          id: "1",
          name: "Sushi",
          price: "15",
          weight: "123",
          imageUrl: "example.com",
        },
        {
          id: "2",
          name: "Ramen",
          price: "13",
          weight: "321",
          imageUrl: "example.com",
        },
      ];
      store.dispatch({
        type: getPopularDishes.fulfilled.type,
        payload: mockPopularDishes,
      });

      expect(store.getState().dishes.dishesLoading).toBe(false);
      expect(store.getState().dishes.popularDishes).toEqual(mockPopularDishes);
    });

    it("should handle getPopularDishes.rejected", () => {
      store.dispatch({ type: getPopularDishes.rejected.type });
      expect(store.getState().dishes.dishesLoading).toBe(false);
    });
  });
});
