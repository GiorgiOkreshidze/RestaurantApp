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
  name: string;
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
