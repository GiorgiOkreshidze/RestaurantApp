import axios, {
  AxiosError,
  AxiosInstance,
  InternalAxiosRequestConfig,
} from "axios";
import { RootState } from "../app/store";
import { Store } from "@reduxjs/toolkit";
import { apiURL } from "./constants";
import { logout, setUser } from "@/app/slices/userSlice";

const axiosApi: AxiosInstance = axios.create({
  baseURL: apiURL,
});

let isRefreshing = false;
let refreshSubscribers: ((token: string) => void)[] = [];

const refreshTokenRequest = async (store: Store<RootState>) => {
  try {
    const refreshToken = store.getState().users.user?.tokens.refreshToken;
    const response = await axios.post(`${apiURL}/auth/refresh`, {
      refreshToken,
    });
    store.dispatch(setUser(response.data));
    return response.data.token;
  } catch (error) {
    store.dispatch(logout());
    throw error;
  }
};

interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

const addInterceptors = (store: Store<RootState>) => {
  axiosApi.interceptors.request.use((config) => {
    const token = store.getState().users.user?.tokens.accessToken;
    if (token) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
  });

  axiosApi.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
      const originalRequest = error.config as
        | CustomAxiosRequestConfig
        | undefined;

      if (
        error.response?.status === 401 &&
        originalRequest &&
        !originalRequest._retry
      ) {
        if (isRefreshing) {
          return new Promise((resolve) => {
            refreshSubscribers.push((token) => {
              originalRequest.headers["Authorization"] = `Bearer ${token}`;
              resolve(axiosApi(originalRequest));
            });
          });
        }

        originalRequest._retry = true;
        isRefreshing = true;

        try {
          const newToken = await refreshTokenRequest(store);
          refreshSubscribers.forEach((callback) => callback(newToken));
          refreshSubscribers = [];
          isRefreshing = false;

          originalRequest.headers["Authorization"] = `Bearer ${newToken}`;
          return axiosApi(originalRequest);
        } catch (err) {
          isRefreshing = false;
          return Promise.reject(err);
        }
      }

      return Promise.reject(error);
    }
  );
};

export { addInterceptors };
export default axiosApi;
