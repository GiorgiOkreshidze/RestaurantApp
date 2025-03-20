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
}

const addInterceptors = (store: Store<RootState>) => {
  axiosApi.interceptors.request.use((config) => {
    const user = store.getState().users.user;
    if (user?.tokens) {
      config.headers["Authorization"] = `Bearer ${user.tokens.idToken}`;
      console.log(config.url);
      if (config.url === "users/profile") {
        config.headers["X-Amz-Security-Token"] = user.tokens.accessToken;
      }
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
            refreshSubscribers.push((tokens) => {
              originalRequest.headers[
                "Authorization"
              ] = `Bearer ${tokens.idToken}`;
              originalRequest.headers["X-Amz-Security-Token"] = tokens.accessToken;
              resolve(axiosApi(originalRequest));
            });
          });
        }

        originalRequest._retry = true;
        isRefreshing = true;

        try {
          const newTokens = await refreshTokenRequest(store);
          refreshSubscribers.forEach((callback) => callback(newTokens));
          refreshSubscribers = [];
          isRefreshing = false;

          originalRequest.headers[
            "Authorization"
          ] = `Bearer ${newTokens.idToken}`;
          originalRequest.headers["X-Amz-Security-Token"] = newTokens.accessToken;

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
