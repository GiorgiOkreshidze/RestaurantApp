import { Route, Routes } from "react-router";
import { Home, Auth, Location } from "./pages";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { NavBar } from "./components/shared";

function App() {
  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} theme="light" />
      <header>
        <NavBar />
      </header>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/signin" element={<Auth />} />
        <Route path="/signup" element={<Auth />} />
        <Route path="/locations/:id" element={<Location />} />
      </Routes>
    </>
  );
}

export default App;
