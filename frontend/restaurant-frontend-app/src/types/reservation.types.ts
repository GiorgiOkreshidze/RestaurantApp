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
  tableId: string;
  tableNumber: string;
  timeFrom: string;
  timeTo: string;
  timeSlot: string;
  userEmail: string;
  userInfo: string;
  createdAt: string;
}

export interface ReservationUpsertRequestParams {
  id?: string;
  locationId: string;
  tableNumber: string;
  date: string /* YYYY-MM-DD */;
  timeFrom: string /* HH:MM (24H) */;
  timeTo: string /* HH:MM (24H) */;
  guestsNumber: string;
  tableId: string;
}

export interface ReservationDialogProps {
  locationAddress: string;
  locationId: string;
  tableId: string;
  tableNumber: string;
  date: Date;
  initTime: string;
  initGuests: number;
  maxGuests: number;
}
