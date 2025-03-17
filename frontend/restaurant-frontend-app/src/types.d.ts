export interface UserFields {
  name: string;
  lastName: string;
  email: string;
  password: string;
}

export interface User {
  id: string;
  name: string;
  lastName: string;
  email: string;
  token: string;
}

export interface RegisterMutation {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface RegisterResponse {
  message: string;
  user: User;
}

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

export interface Dish {
  name: string;
  cost: string;
  weight: string;
  image: string;
}

export interface Review {
  name: string;
  date: string;
  rating: number;
  review: string;
  image: string;
}

export interface LoginFormFields
  extends Pick<UserFields, "email" | "password"> {}
