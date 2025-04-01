import { Route, Routes, useLocation } from "react-router";
import { Home, Auth, Location, Menu } from "./pages";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { NavBar } from "./components/shared";
import { useEffect } from "react";
import { Reservations } from "./pages/Reservations";
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
import { getReservations } from "./app/thunks/reservationsThunks";
import { selectReservations } from "./app/slices/reservationsSlice";
import { ProtectedRoute } from "./components/routeComponents/ProtectedRoute";
import { PublicRoute } from "./components/routeComponents/PublicRoute";



function App() {
  const location = useLocation();
  const hideNavBar = ["/signin", "/signup"].includes(location.pathname);
  const dispatch = useAppDispatch();
  const popularDishes = useSelector(selectPopularDishes);
  const locations = useSelector(selectLocations);
  const selectOptions = useSelector(selectSelectOptions);
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
    if (!selectOptions.length) {
      dispatch(getSelectOptions());
    }
  }, [dispatch, selectOptions.length]);

  useEffect(() => {
    if (!reservations.length) {
      dispatch(getReservations());
    }
  }, [dispatch, reservations.length]);

 
  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} theme="light" />
      {!hideNavBar && (
        <header>
          <NavBar />
        </header>
      )}
      <Routes>
        <Route path="/" element={<Home />} />
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
              <Reservations />
            </ProtectedRoute>
          }
        />
        <Route path="/booking" element={<Booking />} />
      </Routes>
    </>
  );
}

export default App;
