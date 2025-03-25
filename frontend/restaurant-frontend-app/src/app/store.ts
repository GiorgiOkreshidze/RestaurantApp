import { combineReducers, configureStore } from "@reduxjs/toolkit";
import storage from "redux-persist/lib/storage";
import {
  FLUSH,
  PAUSE,
  PERSIST,
  persistReducer,
  persistStore,
} from "redux-persist";
import { PURGE, REGISTER, REHYDRATE } from "redux-persist/es/constants";
import { usersReducer } from "./slices/userSlice";
import { dishesReducer } from "./slices/dishesSlice";
import { locationsReducer } from "./slices/locationsSlice";
import { reservationsReducer } from "./slices/reservationsSlice";
import { tablesReducer } from "./slices/tablesSlice";

const usersPersistConfig = {
  key: "restaurant:users",
  storage: storage,
  whitelist: ["user"],
};

const rootReducer = combineReducers({
  users: persistReducer(usersPersistConfig, usersReducer),
  dishes: dishesReducer,
  locations: locationsReducer,
  reservations: reservationsReducer,
  tables: tablesReducer,
});

export const store = configureStore({
  reducer: rootReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: [FLUSH, PAUSE, PERSIST, REHYDRATE, PURGE, REGISTER],
      },
    }),
});

export const persistor = persistStore(store);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
