import { User } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import { getUserData, login, register, signout } from "../thunks/userThunks";
import { toast } from "react-toastify";

interface UserState {
  user: User | null;
  registerLoading: boolean;
  loginLoading: boolean;
  userDataLoading: boolean;
  signoutLoading: boolean;
}

const initialState: UserState = {
  user: null,
  registerLoading: false,
  loginLoading: false,
  userDataLoading: false,
  signoutLoading: false,
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
        // state.user = {
        //   tokens: {
        //     accessToken: data.accessToken,
        //     idToken: data.idToken,
        //     refreshToken: data.refreshToken,
        //   },
        //   name: state.user?.name || "",
        //   lastName: state.user?.lastName || "",
        //   email: state.user?.email || "",
        //   role: state.user?.role || "",
        //   imageUrl: state.user?.imageUrl || "",
        // };
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
            role: "",
            imageUrl: "",
          };
        }
      })
      .addCase(login.rejected, (state) => {
        state.loginLoading = false;
      });

    builder
      .addCase(getUserData.pending, (state) => {
        state.userDataLoading = true;
      })
      .addCase(getUserData.fulfilled, (state, { payload: data }) => {
        state.userDataLoading = false;
        if (state.user) {
          state.user.name = data.name;
          state.user.lastName = data.lastName;
          state.user.email = data.email;
        }
      })
      .addCase(getUserData.rejected, (state) => {
        state.userDataLoading = false;
      });

    builder
      .addCase(signout.pending, (state) => {
        state.signoutLoading = true;
      })
      .addCase(signout.fulfilled, (state) => {
        state.signoutLoading = false;
        state.user = null;
      })
      .addCase(signout.rejected, (state) => {
        state.signoutLoading = false;
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
