import { beforeEach, describe, expect, it, vi } from "vitest";
import { createAuthRequest, refreshTokenRequest } from "../axiosApi";
import axios, { InternalAxiosRequestConfig } from "axios";
import { RootState } from "@/app/store";
import { Store } from "@reduxjs/toolkit";
import { logout, setUser } from "@/app/slices/userSlice";

vi.mock("axios", () => ({
  default: {
    create: vi.fn(() => ({
      interceptors: {
        request: { use: vi.fn() },
        response: { use: vi.fn() },
      },
    })),
    post: vi.fn().mockResolvedValue({
      data: {
        tokens: { accessToken: "new-access-token", idToken: "new-id-token" },
      },
    }),
  },
}));

const mockStore = {
  getState: vi.fn().mockReturnValue({
    users: { user: { tokens: { refreshToken: "test-refresh-token" } } },
  }),
  dispatch: vi.fn(),
};

describe("axiosApi.ts", () => {
  describe("createAuthRequest", () => {
    it("should add 'skipRefreshToken' property", () => {
      const result = createAuthRequest({} as InternalAxiosRequestConfig);
      expect(result).toEqual({ skipRefreshToken: true });
    });
  });

  describe("refreshTokenRequest", () => {
    beforeEach(() => {
      vi.clearAllMocks();
    });

    it("should return new tokens", async () => {
      const result = await refreshTokenRequest(
        mockStore as unknown as Store<RootState>,
      );
      expect(axios.post).toBeCalledWith(
        expect.stringContaining("/auth/refresh"),
        { refreshToken: "test-refresh-token" },
      );
      expect(mockStore.dispatch).toBeCalledWith(
        setUser(
          expect.objectContaining({
            tokens: expect.objectContaining({
              accessToken: "new-access-token",
              idToken: "new-id-token",
            }),
          }),
        ),
      );
      expect(result).toEqual({
        accessToken: "new-access-token",
        idToken: "new-id-token",
      });
    });

    it("should dispatch logout when tokens refresh fails", async () => {
      vi.mocked(axios.post).mockRejectedValueOnce(new Error("Refresh failed"));
      await expect(
        refreshTokenRequest(mockStore as unknown as Store<RootState>),
      ).rejects.toThrow();
      expect(mockStore.dispatch).toHaveBeenCalledWith(logout());
    });
  });
});
