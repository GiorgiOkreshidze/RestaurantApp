export interface UserFields {
  name: string;
  lastName: string;
  email: string;
  password: string;
}

export interface User {
  tokens: {
    accessToken: string;
    idToken: string;
    refreshToken: string;
  };

  name: string;
  lastName: string;
  email: string;
  role: string;
  imageUrl: string;
}

export interface RegisterMutation {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface LoginMutation {
  email: string;
  password: string;
}

export interface RegisterResponse {
  message: string;
  accessToken: string;
  idToken: string;
  refreshToken: string;
}

export interface LoginResponse {
  accessToken: string;
  idToken: string;
  refreshToken: string;
}

export interface UserDataResponse
  extends Pick<UserFields, "name" | "lastName" | "email"> {}

export interface RegistrationFields extends UserFields {
  confirmPassword: string;
}

export interface ValidationErrors {
  name?: string;
  lastName?: string;
  email?: string;
  password?: string;
  confirmPassword?: string;
}

export interface GlobalErrorMessage {
  message: string;
}

export interface signOutMutation {
  refreshToken: string;
}

export interface Dish {
  id: string;
  name: string;
  price: string;
  weight: string;
  imageUrl: string;
}

export interface Location {
  id: string;
  address: string;
  description: string;
  totalCapacity: string;
  averageOccupancy: string;
  imageUrl: string;
  rating: string;
}

export interface Review {
  name: string;
  date: string;
  rating: number;
  review: string;
  image: string;
}

export type ReservationStatus =
  | "Reserved"
  | "In Progress"
  | "Canceled"
  | "Finished";

export interface Reservation {
  location: string;
  status: ReservationStatus;
  date: string;
  time: string;
  guests: string;
}
