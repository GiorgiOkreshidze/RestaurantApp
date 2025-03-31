import { Route, Routes, useLocation } from "react-router";
import { Home, Auth, Location, WaiterReservation } from "./pages";
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
import { selectUser } from "./app/slices/userSlice";

function App() {
  const location = useLocation();
  const hideNavBar = ["/signin", "/signup"].includes(location.pathname);
  const dispatch = useAppDispatch();
  const popularDishes = useSelector(selectPopularDishes);
  const locations = useSelector(selectLocations);
  const selectOptions = useSelector(selectSelectOptions);
  const reservations = useSelector(selectReservations);
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

  useEffect(() => {
    if (!selectOptions.length) {
      dispatch(getSelectOptions());
    }
  }, [dispatch, selectOptions.length]);

  useEffect(() => {
    if (!selectOptions.length) {
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
        <Route
          path="/"
          element={user?.role === "Waiter" ? <WaiterReservation /> : <Home />}
        />

        <Route path="/signin" element={<Auth />} />
        <Route path="/signup" element={<Auth />} />
        <Route path="/locations/:id" element={<Location />} />
        <Route
          path="/reservations"
          element={
            user?.role === "Waiter" ? <WaiterReservation /> : <Reservations />
          }
        />

        <Route path="/booking" element={<Booking />} />
      </Routes>
    </>
  );
}

export default App;
