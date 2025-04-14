import { Dish } from "@/types";
import { Preorder } from "@/types/preorder.types";
import { createSlice } from "@reduxjs/toolkit";

export interface PreordersState {
  preorders: Preorder[];
  activePreorderId: Preorder["id"] | null;
}

const initialState: PreordersState = {
  preorders: [],
  activePreorderId: null,
};

export const preordersSlice = createSlice({
  name: "preorders",
  initialState,
  reducers: {
    setActivePreorder: (
      state,
      { payload: preorderId }: { payload: Preorder["id"] | null },
    ) => {
      state.activePreorderId = preorderId;
      if (!preorderId) return;
      const isPreorderPresent = state.preorders.some(
        (preorder) => preorder.id === preorderId,
      );
      if (!isPreorderPresent) {
        state.preorders.push({
          id: preorderId,
          status: "new",
          dishes: [],
          number: state.preorders.length,
        });
      }
    },
    setPreorderStatus: (
      state,
      {
        payload,
      }: {
        payload: { preorderId: Preorder["id"]; status: Preorder["status"] };
      },
    ) => {
      const preorder = state.preorders.find(
        (preorder) => preorder.id === payload.preorderId,
      );
      if (preorder) {
        preorder.status = payload.status;
      }
    },
    addDishToActivePreorder: (
      state,
      { payload: dishId }: { payload: Dish["id"] },
    ) => {
      const activePreorder = state.preorders.find(
        (preorder) => preorder.id === state.activePreorderId,
      );
      if (activePreorder) {
        activePreorder.dishes.push({
          id: dishId,
          count: 1,
        });
      }
    },
    deleteDishFromPreorder: (
      state,
      {
        payload,
      }: { payload: { dishId: Dish["id"]; preorderId: Preorder["id"] } },
    ) => {
      const preorder = state.preorders.find(
        (preorder) => preorder.id === payload.preorderId,
      );
      if (preorder) {
        preorder.dishes = preorder.dishes.filter(
          (dish) => dish.id !== payload.dishId,
        );
      }
    },
    increaseDishInPreorder: (
      state,
      {
        payload,
      }: { payload: { dishId: Dish["id"]; preorderId: Preorder["id"] } },
    ) => {
      const preorder = state.preorders.find(
        (preorder) => preorder.id === payload.preorderId,
      );
      const dish = preorder?.dishes.find((dish) => dish.id === payload.dishId);
      if (dish) {
        dish.count += 1;
      }
    },
    decreaseDishInPreorder: (
      state,
      {
        payload,
      }: { payload: { dishId: Dish["id"]; preorderId: Preorder["id"] } },
    ) => {
      const preorder = state.preorders.find(
        (preorder) => preorder.id === payload.preorderId,
      );
      const dish = preorder?.dishes.find((dish) => dish.id === payload.dishId);
      if (dish && dish.count > 1) {
        dish.count -= 1;
      }
    },
  },
  selectors: {
    selectPreorders: (state) => state.preorders,
    selectActivePreorderId: (state) => state.activePreorderId,
    selectActivePreorder: (state) =>
      state.preorders.find(
        (preorder) => preorder.id === state.activePreorderId,
      ),
  },
});

export const preordersReducer = preordersSlice.reducer;
export const {
  setActivePreorder,
  addDishToActivePreorder,
  increaseDishInPreorder,
  decreaseDishInPreorder,
  deleteDishFromPreorder,
  setPreorderStatus,
} = preordersSlice.actions;
export const { selectActivePreorder, selectActivePreorderId, selectPreorders } =
  preordersSlice.selectors;
