import { Route, Routes } from "react-router";
import { Home, Location, Registration } from "./pages";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { NavBar } from "./components/shared";
// import { Login } from "./pages/Login";

function App() {
  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} theme="light" />
      <header>
        <NavBar />
      </header>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/signup" element={<Registration />} />
        <Route path="/locations/:id" element={<Location />} />
        <Route path="/signin" element={<Login />} />
      </Routes>
    </>
  );
}

export default App;
