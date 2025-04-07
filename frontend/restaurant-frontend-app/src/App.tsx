import { Route, Routes, useLocation } from "react-router";
import { Home, Auth, Location, Menu, WaiterReservation } from "./pages";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { NavBar } from "./components/shared";
import { useEffect } from "react";
import { ClientReservations } from "./pages/ClientReservations";
import { Booking } from "./pages/Booking";
import { useSelector } from "react-redux";
import { useAppDispatch } from "./app/hooks";
import { selectPopularDishes } from "./app/slices/dishesSlice";
import {
  selectLocations,
  selectSelectOptions,
} from "./app/slices/locationsSlice";
import { getPopularDishes } from "./app/thunks/dishesThunks";
import { getLocations, getSelectOptions } from "./app/thunks/locationsThunks";
import { ProtectedRoute } from "./components/routeComponents/ProtectedRoute";
import { PublicRoute } from "./components/routeComponents/PublicRoute";

import { USER_ROLE } from "./utils/constants";
import { selectUser } from "./app/slices/userSlice";
import { setLocationAction } from "./app/slices/bookingFormSlice";

function App() {
  const location = useLocation();
  const hideNavBar = ["/signin", "/signup"].includes(location.pathname);
  const dispatch = useAppDispatch();
  const popularDishes = useSelector(selectPopularDishes);
  const locations = useSelector(selectLocations);
    const selectOptions = useSelector(selectSelectOptions);
  const user = useSelector(selectUser);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [location.pathname]);

  useEffect(() => {
    if (!popularDishes.length) {
      dispatch(getPopularDishes());
    }
  }, [dispatch, popularDishes.length]);

  useEffect(() => {
    if (!locations.length) {
      dispatch(getLocations());
    }
  }, [dispatch, locations.length]);

  useEffect(() => {(async () => {
    if (!selectOptions.length) {
      const selectOptions = await dispatch(getSelectOptions()).unwrap();
      dispatch(setLocationAction(selectOptions[0].id));
    }
  })()}, [dispatch, selectOptions.length]);

  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} theme="light" />
      {!hideNavBar && (
        <header>
          <NavBar />
        </header>
      )}
      <Routes>
        <Route
          path="/"
          element={
            user?.role === USER_ROLE.WAITER ? <WaiterReservation /> : <Home />
          }
        />

        <Route
          path="/menu"
          element={
            <ProtectedRoute>
              <Menu />
            </ProtectedRoute>
          }
        />
        <Route
          path="/signin"
          element={
            <PublicRoute>
              <Auth />
            </PublicRoute>
          }
        />
        <Route
          path="/signup"
          element={
            <PublicRoute>
              <Auth />
            </PublicRoute>
          }
        />
        <Route path="/locations/:id" element={<Location />} />
        <Route
          path="/reservations"
          element={
            <ProtectedRoute>
              <ClientReservations />
            </ProtectedRoute>
          }
        />
        <Route path="/booking" element={<Booking />} />
      </Routes>
    </>
  );
}

export default App;
