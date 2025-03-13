import { User } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import { register } from "../thunks/userThunks";
import { toast } from "react-toastify";

interface UserState {
  user: User | null;
  registerLoading: boolean;
}

const initialState: UserState = {
  user: null,
  registerLoading: false,
};

export const userSlice = createSlice({
  name: "users",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(register.pending, (state) => {
        state.registerLoading = true;
      })
      .addCase(register.fulfilled, (state, { payload: data }) => {
        state.registerLoading = false;
        state.user = data.user;
        toast.success(data.message);
      })
      .addCase(register.rejected, (state, { payload: errorResponse }) => {
        state.registerLoading = false;
        toast.error(errorResponse?.message);
      });
  },
  selectors: {
    selectUser: (state) => state.user,
    selectRegisterLoading: (state) => state.registerLoading,
  },
});

export const usersReducer = userSlice.reducer;
export const { selectUser, selectRegisterLoading } = userSlice.selectors;
