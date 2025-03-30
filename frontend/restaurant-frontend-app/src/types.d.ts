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

export interface Review {
  name: string;
  date: string;
  rating: number;
  review: string;
  image: string;
}

export interface RichTimeSlot {
  id: string;
  startString: string;
  endString: string;
  rangeString: string;
  startDate: string;
  endDate: string;
  isPast: boolean;
}
