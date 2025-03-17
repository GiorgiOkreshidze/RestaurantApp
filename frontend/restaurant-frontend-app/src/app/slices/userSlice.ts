import { User } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import { login, register } from "../thunks/userThunks";
import { toast } from "react-toastify";

interface UserState {
  user: User | null;
  registerLoading: boolean;
  loginLoading: boolean;
}

const initialState: UserState = {
  user: null,
  registerLoading: false,
  loginLoading: false,
};

export const userSlice = createSlice({
  name: "users",
  initialState,
  reducers: {
    setUser: (state, { payload: user }) => {
      state.user = user;
    },
    logout: (state) => {
      state.user = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(register.pending, (state) => {
        state.registerLoading = true;
      })
      .addCase(register.fulfilled, (state, { payload: data }) => {
        state.registerLoading = false;
        state.user = {
          tokens: {
            accessToken: data.accessToken,
            idToken: data.idToken,
            refreshToken: data.refreshToken,
          },
          name: state.user?.name || "",
          lastName: state.user?.lastName || "",
          email: state.user?.email || "",
        };

        toast.success(data.message);
      })
      .addCase(register.rejected, (state, { payload: errorResponse }) => {
        state.registerLoading = false;
        toast.error(errorResponse?.message);
      });

    builder
      .addCase(login.pending, (state) => {
        state.loginLoading = true;
      })
      .addCase(login.fulfilled, (state, { payload: data }) => {
        state.loginLoading = false;
        if (state.user) {
          state.user.tokens = data;
        } else {
          state.user = {
            tokens: data,
            name: "",
            lastName: "",
            email: "",
          };
        }
      })
      .addCase(login.rejected, (state) => {
        state.loginLoading = false;
      });
  },
  selectors: {
    selectUser: (state) => state.user,
    selectRegisterLoading: (state) => state.registerLoading,
    selectLoginLoading: (state) => state.loginLoading,
  },
});

export const usersReducer = userSlice.reducer;
export const { setUser, logout } = userSlice.actions;
export const { selectUser, selectRegisterLoading, selectLoginLoading } =
  userSlice.selectors;
