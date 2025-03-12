import { Route, Routes } from "react-router";
import Registration from "./pages/Registration";

function App() {
  return (
    <>
      <Routes>
        <Route path="/registration" element={<Registration />} />
      </Routes>
    </>
  );
}

export default App;
