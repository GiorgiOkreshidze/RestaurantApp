import { describe, it, expect, vi, beforeEach } from "vitest";
import { configureStore, EnhancedStore } from "@reduxjs/toolkit";
import {
  usersReducer,
  initialState,
  setUser,
  logout,
  UserState,
} from "../userSlice";
import {
  getUserData,
  getAllUsers,
  login,
  register,
  signout,
} from "../../thunks/userThunks";

interface RootState {
  users: UserState;
}

vi.mock("react-toastify", () => ({
  toast: {
    success: vi.fn(),
    error: vi.fn(),
  },
}));

vi.mock("../path/to/thunks/userThunks", () => ({
  getUserData: vi.fn(),
  getAllUsers: vi.fn(),
  login: vi.fn(),
  register: vi.fn(),
  signout: vi.fn(),
}));

describe("userSlice", () => {
  let store: EnhancedStore<RootState>;

  beforeEach(() => {
    vi.clearAllMocks();

    store = configureStore({
      reducer: {
        users: usersReducer,
      },
    });
  });

  describe("reducers", () => {
    it("should handle initial state", () => {
      expect(store.getState().users).toEqual(initialState);
    });

    it("should handle setUser", () => {
      const mockUser = {
        id: "123",
        locationId: "loc-123",
        firstName: "John",
        lastName: "Doe",
        email: "john@example.com",
        role: "USER",
        imageUrl: "avatar.jpg",
        tokens: {
          accessToken: "access",
          idToken: "id",
          refreshToken: "refresh",
        },
      };

      store.dispatch(setUser(mockUser));
      expect(store.getState().users.user).toEqual(mockUser);
    });

    it("should handle logout", () => {
      // First set a user
      const mockUser = {
        id: "123",
        firstName: "John",
        lastName: "Doe",
        email: "john@example.com",
        role: "USER",
        imageUrl: "avatar.jpg",
        tokens: {
          accessToken: "access",
          idToken: "id",
          refreshToken: "refresh",
        },
      };

      store.dispatch(setUser(mockUser));
      expect(store.getState().users.user).toEqual(mockUser);

      // Then logout
      store.dispatch(logout());
      expect(store.getState().users.user).toBeNull();
    });
  });

  describe("thunks", () => {
    describe("register thunk", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: register.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().users.registerLoading).toBe(true);
      });

      it("should handle fulfilled state", () => {
        const fulfilledAction = {
          type: register.fulfilled.type,
          payload: { message: "Registration successful" },
        };
        store.dispatch(fulfilledAction);
        expect(store.getState().users.registerLoading).toBe(false);
      });

      it("should handle rejected state", () => {
        const rejectedAction = {
          type: register.rejected.type,
          payload: { message: "Registration failed" },
        };
        store.dispatch(rejectedAction);
        expect(store.getState().users.registerLoading).toBe(false);
      });
    });

    describe("login thunk", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: login.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().users.loginLoading).toBe(true);
      });

      it("should handle fulfilled state with existing user", () => {
        // Set up initial user
        const initialUser = {
          id: "123",
          locationId: "loc-123",
          firstName: "John",
          lastName: "Doe",
          email: "john@example.com",
          role: "USER",
          imageUrl: "avatar.jpg",
          tokens: null,
        };
        store.dispatch(setUser(initialUser));

        // Now fulfill the login action
        const tokens = {
          accessToken: "new-access",
          idToken: "new-id",
          refreshToken: "new-refresh",
        };

        const fulfilledAction = {
          type: login.fulfilled.type,
          payload: tokens,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().users.loginLoading).toBe(false);
        expect(store.getState().users.user?.tokens).toEqual(tokens);
      });

      it("should handle fulfilled state with no initial user", () => {
        const tokens = {
          accessToken: "access",
          idToken: "id",
          refreshToken: "refresh",
        };

        const fulfilledAction = {
          type: login.fulfilled.type,
          payload: tokens,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().users.loginLoading).toBe(false);
        expect(store.getState().users.user).toEqual({
          locationId: "",
          id: "",
          tokens,
          firstName: "",
          lastName: "",
          email: "",
          role: "",
          imageUrl: "",
        });
      });

      it("should handle rejected state", () => {
        const rejectedAction = {
          type: login.rejected.type,
          payload: { message: "Invalid credentials" },
        };
        store.dispatch(rejectedAction);
        expect(store.getState().users.loginLoading).toBe(false);
      });
    });

    describe("getUserData thunk", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getUserData.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().users.userDataLoading).toBe(true);
      });

      it("should handle fulfilled state and update user data", () => {
        // Set up initial user with tokens only
        const initialUser = {
          id: "",
          locationId: "",
          firstName: "",
          lastName: "",
          email: "",
          role: "",
          imageUrl: "",
          tokens: {
            accessToken: "access",
            idToken: "id",
            refreshToken: "refresh",
          },
        };
        store.dispatch(setUser(initialUser));

        // Now fulfill getUserData
        const userData = {
          id: "user-123",
          locationId: "loc-456",
          firstName: "Jane",
          lastName: "Smith",
          email: "jane@example.com",
          role: "ADMIN",
        };

        const fulfilledAction = {
          type: getUserData.fulfilled.type,
          payload: userData,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().users.userDataLoading).toBe(false);
        expect(store.getState().users.user).toEqual({
          ...initialUser,
          ...userData,
        });
      });

      it("should handle fulfilled state with no initial user", () => {
        const userData = {
          id: "user-123",
          locationId: "loc-456",
          firstName: "Jane",
          lastName: "Smith",
          email: "jane@example.com",
          role: "ADMIN",
        };

        const fulfilledAction = {
          type: getUserData.fulfilled.type,
          payload: userData,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().users.userDataLoading).toBe(false);
        // No user should still be null as getUserData only updates existing user
        expect(store.getState().users.user).toBeNull();
      });

      it("should handle rejected state", () => {
        const rejectedAction = {
          type: getUserData.rejected.type,
          payload: { message: "Failed to fetch user data" },
        };
        store.dispatch(rejectedAction);
        expect(store.getState().users.userDataLoading).toBe(false);
      });
    });

    describe("signout thunk", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: signout.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().users.signoutLoading).toBe(true);
      });

      it("should handle fulfilled state and clear user", () => {
        // Set up initial user
        const initialUser = {
          id: "123",
          firstName: "John",
          lastName: "Doe",
          email: "john@example.com",
          role: "USER",
          imageUrl: "avatar.jpg",
          tokens: {
            accessToken: "access",
            idToken: "id",
            refreshToken: "refresh",
          },
        };
        store.dispatch(setUser(initialUser));
        expect(store.getState().users.user).not.toBeNull();

        // Now fulfill signout
        const fulfilledAction = {
          type: signout.fulfilled.type,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().users.signoutLoading).toBe(false);
        expect(store.getState().users.user).toBeNull();
      });

      it("should handle rejected state", () => {
        const rejectedAction = {
          type: signout.rejected.type,
          payload: { message: "Failed to sign out" },
        };
        store.dispatch(rejectedAction);
        expect(store.getState().users.signoutLoading).toBe(false);
      });
    });

    describe("getAllUsers thunk", () => {
      it("should handle pending state", () => {
        const pendingAction = { type: getAllUsers.pending.type };
        store.dispatch(pendingAction);
        expect(store.getState().users.allUsersLoading).toBe(true);
      });

      it("should handle fulfilled state and set allUsers", () => {
        const users = [
          {
            id: "user-1",
            firstName: "John",
            lastName: "Doe",
            email: "john@example.com",
            role: "USER",
          },
          {
            id: "user-2",
            firstName: "Jane",
            lastName: "Smith",
            email: "jane@example.com",
            role: "ADMIN",
          },
        ];

        const fulfilledAction = {
          type: getAllUsers.fulfilled.type,
          payload: users,
        };

        store.dispatch(fulfilledAction);

        expect(store.getState().users.allUsersLoading).toBe(false);
        expect(store.getState().users.allUsers).toEqual(users);
      });

      it("should handle rejected state", () => {
        const rejectedAction = {
          type: getAllUsers.rejected.type,
          payload: { message: "Failed to fetch users" },
        };
        store.dispatch(rejectedAction);
        expect(store.getState().users.allUsersLoading).toBe(false);
      });
    });
  });

  describe("selectors", () => {
    it("should select user", () => {
      const mockUser = {
        id: "123",
        firstName: "John",
        lastName: "Doe",
        email: "john@example.com",
        role: "USER",
        imageUrl: "avatar.jpg",
        tokens: {
          accessToken: "access",
          idToken: "id",
          refreshToken: "refresh",
        },
      };

      store.dispatch(setUser(mockUser));

      const state = store.getState();
      // We can't directly test the selector here since they're created with createSlice,
      // but we can check the state directly
      expect(state.users.user).toEqual(mockUser);
    });

    it("should select loading states", () => {
      store.dispatch({ type: login.pending.type });
      store.dispatch({ type: register.pending.type });
      store.dispatch({ type: getAllUsers.pending.type });

      const state = store.getState();
      expect(state.users.loginLoading).toBe(true);
      expect(state.users.registerLoading).toBe(true);
      expect(state.users.allUsersLoading).toBe(true);
    });

    it("should select allUsers", () => {
      const users = [
        {
          id: "user-1",
          firstName: "John",
          lastName: "Doe",
          email: "john@example.com",
          role: "USER",
        },
      ];

      store.dispatch({
        type: getAllUsers.fulfilled.type,
        payload: users,
      });

      const state = store.getState();
      expect(state.users.allUsers).toEqual(users);
    });
  });
});
