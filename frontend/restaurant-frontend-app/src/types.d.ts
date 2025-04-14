export interface UserFields {
  name: string;
  lastName: string;
  email: string;
  password: string;
}

export type UserRole = "Waiter" | "Customer";

export interface User {
  tokens: {
    accessToken: string;
    idToken: string;
    refreshToken: string;
  };

  id: string;
  locationId?: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
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

export type UserDataResponse = Omit<User, "tokens">;

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
  state?: string;
}

export interface ExtendedDish extends Dish {
  description: string;
  calories: string;
  carbohydrates: string;
  fats: string;
  proteins: string;
  vitamins: string;
}

type CategoryType = "Appetizers" | "MainCourse" | "Desserts" | "";

type SortOptionType = {
  id: string;
  label: string;
};

export interface FeedbacksResponse {
  content: Review[];
}

export interface Review {
  id: string;
  userName: string;
  userAvatarUrl: string;
  date: string;
  rate: number;
  comment: string;
  date: string;
  type: string;
  locationId: string;
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

export interface LocationTable {
  tableId: string;
  tableNumber: string;
  capacity: string;
  locationId: string;
  locationAddress: string;
}
