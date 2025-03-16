import { Route, Routes } from "react-router";
import { Home, Auth } from "./pages";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

function App() {
  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} theme="light" />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/signin" element={<Auth />} />
        <Route path="/signup" element={<Auth />} />
      </Routes>
    </>
  );
}

export default App;
