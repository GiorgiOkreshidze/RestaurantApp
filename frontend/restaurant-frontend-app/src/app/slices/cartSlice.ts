import { createSlice } from "@reduxjs/toolkit";

interface CartState {
  isCartDialogOpen: boolean;
}

const initialState: CartState = {
  isCartDialogOpen: false,
};

const cartSlice = createSlice({
  name: "cart",
  initialState,
  reducers: {
    setIsCartDialogOpen: (
      state,
      { payload: isOpen }: { payload: CartState["isCartDialogOpen"] },
    ) => {
      state.isCartDialogOpen = isOpen;
    },
  },
  selectors: {
    selectIsCartDialogOpen: (state) => state.isCartDialogOpen,
  }
});

export const cartReducer = cartSlice.reducer;
export const { setIsCartDialogOpen } = cartSlice.actions;
export const { selectIsCartDialogOpen } =
  cartSlice.selectors;