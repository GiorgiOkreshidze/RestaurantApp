import { User } from "@/types";
import { createSlice } from "@reduxjs/toolkit";

interface UserState {
  user: User | null;
  isUserLoading: boolean;
}

const initialState: UserState = {
  user: null,
  isUserLoading: false,
};

export const userSlice = createSlice({
  name: "users",
  initialState,
  reducers: {},
  extraReducers: (builder) => {},
  selectors: {
    selectUser: (state) => state.user,
    selectIsUserLoading: (state) => state.isUserLoading,
  },
});

export const usersReducer = userSlice.reducer;
export const { selectUser, selectIsUserLoading } = userSlice.selectors;
