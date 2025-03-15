import { Route, Routes } from "react-router";
import { Home, Login, Registration } from "./pages";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

function App() {
  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} theme="light" />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/signup" element={<Registration />} />
        <Route path="/signin" element={<Login />} />
      </Routes>
    </>
  );
}

export default App;
