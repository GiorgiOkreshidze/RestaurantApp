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

  firstName: string;
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

export interface UserDataResponse extends Omit<User, "tokens"> {}

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
  id: string;
  date: string;
  feedbackId: string;
  guestsNumber: string;
  locationAddress: string;
  locationId: string;
  preOrder: string;
  status: ReservationStatus;
  tableNumber: string;
  timeFrom: string;
  timeTo: string;
  timeSlot: string;
  userInfo: string;
  createdAt: string;
}

export interface UpsertReservationRequest {
  id?: string;
  locationId: string;
  tableNumber: string;
  date: string /* YYYY-MM-DD */;
  timeFrom: string /* HH:MM (24H) */;
  timeTo: string /* HH:MM (24H) */;
  guestsNumber: string;
  tableId: string;
}

export interface SelectOption {
  id: string;
  address: string;
}

export interface Table {
  availableSlots: AvailableSlots[];
  capacity: string;
  locationAddress: string;
  locationId: string;
  tableId: string;
  tableNumber: string;
}

export interface AvailableSlots {
  start: string;
  end: string;
}

export interface TableRequestParams {
  locationId: string;
  date: string /* YYYY-MM-DD */;
  time?: string /* HH:MM (24H) */;
  guests?: string;
}
