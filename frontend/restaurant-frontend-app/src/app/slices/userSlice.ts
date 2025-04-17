import { User, UserDataResponse } from "@/types";
import { createSlice } from "@reduxjs/toolkit";
import {
  getUserData,
  getAllUsers,
  login,
  register,
  signout,
} from "../thunks/userThunks";
import { toast } from "react-toastify";

export interface UserState {
  user: User | null;
  registerLoading: boolean;
  loginLoading: boolean;
  userDataLoading: boolean;
  signoutLoading: boolean;
  allUsers: UserDataResponse[] | null;
  allUsersLoading: boolean;
}

export const initialState: UserState = {
  user: null,
  registerLoading: false,
  loginLoading: false,
  userDataLoading: false,
  signoutLoading: false,
  allUsers: null,
  allUsersLoading: false,
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
            locationId: "",
            id: "",
            tokens: data,
            firstName: "",
            lastName: "",
            email: "",
            role: "",
            imageUrl: "",
          };
        }
        toast.success("Successfully logged in!");
      })
      .addCase(login.rejected, (state, { payload: errorResponse }) => {
        state.loginLoading = false;
        toast.error(errorResponse?.message);
      });

    builder
      .addCase(getUserData.pending, (state) => {
        state.userDataLoading = true;
      })
      .addCase(getUserData.fulfilled, (state, { payload: data }) => {
        state.userDataLoading = false;
        if (state.user) {
          state.user.id = data.id;
          state.user.locationId = data.locationId;
          state.user.firstName = data.firstName;
          state.user.lastName = data.lastName;
          state.user.email = data.email;
          state.user.role = data.role;
        }
      })
      .addCase(getUserData.rejected, (state, { payload: errorResponse }) => {
        state.userDataLoading = false;
        toast.error(errorResponse?.message);
      });

    builder
      .addCase(signout.pending, (state) => {
        state.signoutLoading = true;
      })
      .addCase(signout.fulfilled, (state) => {
        state.signoutLoading = false;
        state.user = null;
      })
      .addCase(signout.rejected, (state, { payload: errorResponse }) => {
        state.signoutLoading = false;
        toast.error(errorResponse?.message);
      });

    builder
      .addCase(getAllUsers.pending, (state) => {
        state.allUsersLoading = true;
      })
      .addCase(getAllUsers.fulfilled, (state, { payload: data }) => {
        state.allUsersLoading = false;
        state.allUsers = data;
      })
      .addCase(getAllUsers.rejected, (state, { payload: errorResponse }) => {
        state.allUsersLoading = false;
        toast.error(errorResponse?.message);
      });
  },
  selectors: {
    selectUser: (state) => state.user,
    selectLoginLoading: (state) => state.loginLoading,
    selectRegisterLoading: (state) => state.registerLoading,
    selectAllUsers: (state) => state.allUsers,
    selectAllUsersLoading: (state) => state.allUsersLoading,
  },
});

export const usersReducer = userSlice.reducer;
export const { setUser, logout } = userSlice.actions;
export const {
  selectUser,
  selectRegisterLoading,
  selectLoginLoading,
  selectAllUsers,
  selectAllUsersLoading,
} = userSlice.selectors;
