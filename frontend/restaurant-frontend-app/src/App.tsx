import { Route, Routes, useLocation } from "react-router";
import { Home, Auth, Location, Menu, WaiterReservation } from "./pages";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { CartDialog, NavBar } from "./components/shared";
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
import {
  getLocations,
  getLocationTables,
  getSelectOptions,
} from "./app/thunks/locationsThunks";
import { ProtectedRoute } from "./components/routeComponents/ProtectedRoute";
import { PublicRoute } from "./components/routeComponents/PublicRoute";

import { selectUser } from "./app/slices/userSlice";
import { selectBooking, setLocationAction } from "./app/slices/bookingSlice";
import { getTimeSlots } from "./app/thunks/bookingThunk";
import { selectReservations } from "./app/slices/reservationsSlice";
import { getReservations } from "./app/thunks/reservationsThunks";
import { getAllUsers } from "./app/thunks/userThunks";

function App() {
  const location = useLocation();
  const hideNavBar = ["/signin", "/signup"].includes(location.pathname);
  const dispatch = useAppDispatch();
  const popularDishes = useSelector(selectPopularDishes);
  const locations = useSelector(selectLocations);
  const selectOptions = useSelector(selectSelectOptions);
  const user = useSelector(selectUser);
  const booking = useSelector(selectBooking);
  const reservations = useSelector(selectReservations);

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

  useEffect(() => {
    (async () => {
      if (!selectOptions.length) {
        const selectOptions = await dispatch(getSelectOptions()).unwrap();
        dispatch(setLocationAction(selectOptions[0].id));
      }
    })();
  }, [dispatch, selectOptions.length]);

  useEffect(() => {
    if (!booking.timeSlots.length) {
      dispatch(getTimeSlots());
      dispatch(getLocationTables());
    }
  }, [dispatch, booking.timeSlots]);

  useEffect(() => {
    if (!user?.role) return;
    if (reservations.length) return;
    dispatch(getReservations({}));
    if (user?.role === "Waiter") {
      dispatch(getAllUsers());
    }
  }, [dispatch, reservations.length, user]);

  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} theme="light" />
      <CartDialog />
      {!hideNavBar && (
        <header>
          <NavBar />
        </header>
      )}
      <Routes>
        <Route
          path="/"
          element={user?.role === "Waiter" ? <WaiterReservation /> : <Home />}
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
