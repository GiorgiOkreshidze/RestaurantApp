import { Route, Routes, useLocation } from "react-router";
import { Home, Auth, Location } from "./pages";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { NavBar } from "./components/shared";
import { useEffect } from "react";
import { Reservations } from "./pages/Reservations";
import { Booking } from "./pages/Booking";
import { useSelector } from "react-redux";
import { useAppDispatch } from "./app/hooks";
import { selectPopularDishes } from "./app/slices/dishesSlice";
import { selectLocations } from "./app/slices/locationsSlice";
import { getPopularDishes } from "./app/thunks/dishesThunks";
import { getLocations } from "./app/thunks/locationsThunks";

function App() {
  const location = useLocation();
  const hideNavBar = ["/signin", "/signup"].includes(location.pathname);
  const dispatch = useAppDispatch();
  const popularDishes = useSelector(selectPopularDishes);
  const locations = useSelector(selectLocations);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [location]);

  useEffect(() => {
    if (!popularDishes.length) {
      dispatch(getPopularDishes());
    }
  }, [dispatch, popularDishes.length]);

  useEffect(() => {
    console.log("test");
    if (!locations.length) {
      dispatch(getLocations());
    }
  }, [dispatch, locations.length]);

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
        <Route path="/signin" element={<Auth />} />
        <Route path="/signup" element={<Auth />} />
        <Route path="/locations/:id" element={<Location />} />
        <Route path="/reservations" element={<Reservations />} />
        <Route path="/booking" element={<Booking />} />
      </Routes>
    </>
  );
}

export default App;
