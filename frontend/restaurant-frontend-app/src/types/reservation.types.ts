import { UserType } from "./user.types";

export type ReservationStatus =
  | "Reserved"
  | "In Progress"
  | "Cancelled"
  | "Finished"
  | "On Stop";

export interface Reservation {
  id: string;
  date: string;
  feedbackId: string;
  guestsNumber: string;
  locationAddress: string;
  locationId: string;
  preOrder: string;
  status: ReservationStatus;
  tableId: string;
  tableNumber: string;
  timeFrom: string;
  timeTo: string;
  timeSlot: string;
  userEmail: string;
  userInfo: string;
  createdAt: string;
  editableTill: string;
}

export interface UpsertReservationRequestParams {
  id?: string;
  locationId: string;
  tableNumber: string;
  date: string /* YYYY-MM-DD */;
  timeFrom: string /* HH:MM (24H) */;
  timeTo: string /* HH:MM (24H) */;
  guestsNumber: string;
  tableId: string;
}

export interface UpsertWaiterReservationRequestParams
  extends UpsertReservationRequestParams {
  clientType: UserType;
  customerName?: string;
  customerId: string;
}

export interface GetReservationRequestParams {
  date?: string /* YYYY-MM-DD */;
  timeFrom?: string /* HH:MM (24H) */;
  tableNumber?: string;
}
