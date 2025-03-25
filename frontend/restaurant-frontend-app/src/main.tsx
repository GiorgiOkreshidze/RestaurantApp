// import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./styles/index.css";
import App from "./App";
import { BrowserRouter } from "react-router";
import { Provider } from "react-redux";
import { store } from "./app/store.ts";
import { addInterceptors } from "./utils/axiosApi.ts";
import { NuqsAdapter } from "nuqs/adapters/react-router/v7";

addInterceptors(store);

createRoot(document.getElementById("root")!).render(
  // <StrictMode>
  <BrowserRouter>
    <Provider store={store}>
      <NuqsAdapter>
        <App />
      </NuqsAdapter>
    </Provider>
  </BrowserRouter>,
  // </StrictMode>,
);
