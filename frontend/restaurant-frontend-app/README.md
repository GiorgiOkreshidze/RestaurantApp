# Restaurant Booking Application - Frontend Documentation

## Overview

This documentation covers the frontend portion of the "Green & Tasty" Restaurant Booking Application, developed as part of Run 7 of the Project Education activity. The application aims to automate and enhance operations across our restaurant network in Tbilisi, Georgia.

## Tech Stack

- **Framework**: React 18
- **Build Tool**: Vite
- **Routing**: React Router v6
- **State Management**: Redux (with Redux Toolkit)
- **API Communication**: Axios
- **Styling**: Tailwind CSS
- **UI Components**: Shadcn UI

## Table of Contents

1. [Getting Started](#getting-started)
2. [Project Structure](#project-structure)
3. [Components](#components)
4. [API Requests](#api-requests)
5. [State Management](#state-management)
6. [Booking Flow](#booking-flow)
7. [Testing](#testing)
8. [Deployment](#deployment)
9. [Contact](#Contact)

## Getting Started

### Prerequisites

- Node.js (v18.0.0 or higher)
- npm (v8.0.0 or higher) or yarn (v1.22.0 or higher)

### Installation

1. Clone the repository:
   ```bash
   git clone https://git.epam.com/epm-edai/project-runs/run-7/team-2/serverless/restaurant-app.git
   cd restaurant-app\automation-qa
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Set up environment variables:
   ```bash
   cp .env.example .env.local
   ```
   Edit `.env` file with your configuration.

4. Start the development server:
   ```bash
   npm run dev
   ```

The application will be available at `http://localhost:5173` by default.

## Project Structure

```
src/
├── assets/            # Static assets (images, fonts, etc.)
├── components/        # Reusable UI components
├── hooks/             # Custom React hooks
├── utils/             # Utility libraries and helpers
├── pages/             # Page components
├── app/               # Redux store and requests
├── styles/            # Global styles and Tailwind utilities
├── App.jsx            # Main App component
└── main.jsx           # Application entry point
```

## Components

### Main component `src/App.tsx`

The primary application component with routing

```jsx
function App() {
  return (
    <>
      <Routes>
        <Route path="/signin" element={
            <PublicRoute>
              <Auth />
            </PublicRoute>
          }
        />
        ...
      </Routes>
    </>
  );
}
```

### Booking page component `src/pages/Booking.tsx`

Should fetch and show list of founded tables


```jsx
  export const Booking = () => {
    const tables = useSelector(selectTables);

    return (
      <PageHero variant="dark" className="flex flex-col justify-center">
        <Text variant="h2" className="text-primary">
          Green & Tasty Restaurants
        </Text>
        <BookingForm className="mt-[2.5rem]" />
      </PageHero>
      <PageBody>
        ...
      </PageBody>
    )
  }
```


## API requests

The application uses Axios for API communication, organized into Redux thunks.

### API Configuration (`src/utils/axiosApi.ts`)

Configures the Axios instance with base settings and request interceptor

```javascript
import axios from 'axios';

const axiosApi: AxiosInstance = axios.create({
  baseURL: apiURL,
});

const addInterceptors = (store: Store<RootState>) => {
  axiosApi.interceptors.request.use((config) => {
    const user = store.getState().users.user;
    if (user?.tokens) {
      config.headers["Authorization"] = `Bearer ${user.tokens.idToken}`;
    }
    return config;
  });
}
```

### API Handler (`src/app/thunks/userThunks.ts`)

An example of the "User signup" Redux Thunk handler

```javascript
export const register = createAsyncThunk<
  RegisterResponse,
  RegisterMutation,
  { rejectValue: GlobalErrorMessage }
>("users/signup", async (registerMutation, { rejectWithValue }) => {
  try {
    const response = await axiosApi.post(serverRoute.signUp, registerMutation);
    return response.data;
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
});
```

### API Error Handling

All API calls are wrapped in try-catch blocks with an error handling:

```javascript
export const requestExample = () => {
  try {
    // ...
  } catch (e) {
    if (isAxiosError(e) && e.response) {
      return rejectWithValue(e.response.data);
    }
    throw e;
  }
};
```

## State Management

The application uses Redux with Redux Toolkit for state management.

### Configuration `src/app/store.ts`

```javascript
const rootReducer = combineReducers({
  users: persistReducer(usersPersistConfig, usersReducer),
  dishes: dishesReducer,
  ...
})

export const store = configureStore({
  reducer: rootReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: [FLUSH, PAUSE, PERSIST, REHYDRATE, PURGE, REGISTER],
      },
    }),
});

export default store;
```

## Booking Flow

The booking process involves several steps:

1. **Date Selection**
   - User selects a date with a calendar component

2. **Time slot Selection**
   - User selects time slot from the predefined list

3. **Guests number**
   - User enters or confirms contact information

4. **Available tables list**
   - User receives a list of available talbes for chosen date and time
   - User confirms booking

5. **Booking Submission**
   - Booking is submitted to the backend
   - Confirmating toast pops up


### Edge Cases in Booking Flow

- **Dates Availability**: The date picker component allows choosing only the future dates
- **Times Availability**: The time picker component allow choosing only future time slots
- **Guests number**: Has a limit of 10 users

## Testing (todo)

## Deployment

### Build Process

To build the application for production:

```bash
npm run build
```

This generates optimized assets in the `dist` directory.

### Deployment Checklist

1. Run all tests: `npm run test`
2. Build the application: `npm run build`
3. Preview the production build: `npm run preview`
4. Upload at Amplify

## Contact

- Team Lead: [Darja Leonova](mailto:darja_leonova@epam.com)