import axios, {
  AxiosError,
  AxiosInstance,
  InternalAxiosRequestConfig,
} from "axios";
import { RootState } from "../app/store";
import { Store } from "@reduxjs/toolkit";
import { apiURL, apiURLLocal } from "./constants";
import { logout, setUser } from "@/app/slices/userSlice";

const axiosApi: AxiosInstance = axios.create({
  baseURL: apiURL,
});

export const axiosLOCAL: AxiosInstance = axios.create({
  baseURL: apiURLLocal,
});

let isRefreshing = false;
let refreshSubscribers: ((tokens: {
  accessToken: string;
  idToken: string;
}) => void)[] = [];

const refreshTokenRequest = async (store: Store<RootState>) => {
  try {
    const refreshToken = store.getState().users.user?.tokens.refreshToken;
    const response = await axios.post(`${apiURL}/auth/refresh`, {
      refreshToken,
    });

    store.dispatch(setUser(response.data));

    return {
      accessToken: response.data.tokens.accessToken,
      idToken: response.data.tokens.idToken,
    };
  } catch (error) {
    store.dispatch(logout());
    throw error;
  }
};

interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
  skipRefreshToken?: boolean;
}

const addInterceptors = (store: Store<RootState>) => {
  axiosApi.interceptors.request.use((config) => {
    const user = store.getState().users.user;
    if (user?.tokens) {
      config.headers["Authorization"] = `Bearer ${user.tokens.idToken}`;
      config.headers["X-Amz-Security-Token"] = user.tokens.accessToken;
    }
    return config;
  });

  axiosApi.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
      console.log(
        "Interceptor caught error:",
        error.response?.status,
        error.config?.url,
      );

      const originalRequest = error.config as
        | CustomAxiosRequestConfig
        | undefined;

      const isAuthRequest = originalRequest?.url?.includes("signin");

      if (
        error.response?.status === 401 &&
        originalRequest &&
        !originalRequest._retry &&
        !isAuthRequest &&
        !originalRequest.skipRefreshToken
      ) {
        console.log("Token expired. Refreshing...");
        originalRequest._retry = true;

        if (isRefreshing) {
          return new Promise((resolve) => {
            refreshSubscribers.push((tokens) => {
              originalRequest.headers["Authorization"] =
                `Bearer ${tokens.idToken}`;
              originalRequest.headers["X-Amz-Security-Token"] =
                tokens.accessToken;
              resolve(axiosApi(originalRequest));
            });
          });
        }

        isRefreshing = true;

        try {
          const newTokens = await refreshTokenRequest(store);
          refreshSubscribers.forEach((callback) => callback(newTokens));
          refreshSubscribers = [];
          isRefreshing = false;

          originalRequest.headers["Authorization"] =
            `Bearer ${newTokens.idToken}`;
          originalRequest.headers["X-Amz-Security-Token"] =
            newTokens.accessToken;

          return axiosApi(originalRequest);
        } catch (err) {
          isRefreshing = false;
          console.error("Refresh token request failed. Logging out.");
          return Promise.reject(err);
        }
      }

      return Promise.reject(error);
    },
  );
};

const createAuthRequest = (
  config: InternalAxiosRequestConfig,
): CustomAxiosRequestConfig => {
  return {
    ...config,
    skipRefreshToken: true,
  };
};

export { addInterceptors, createAuthRequest };
export default axiosApi;
