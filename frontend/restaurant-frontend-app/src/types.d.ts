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
