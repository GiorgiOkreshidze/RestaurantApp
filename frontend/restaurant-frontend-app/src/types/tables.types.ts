import type { RichTimeSlot } from "@/types";

export interface TableServer {
  tableId: string;
  tableNumber: string;
  availableSlots: { start: string; end: string }[];
  capacity: string;
  locationAddress: string;
  locationId: string;
}

export interface TableUI extends Omit<TableServer, "availableSlots"> {
  availableSlots: RichTimeSlot[];
  date: Date;
}

export interface TablesRequestParams {
  locationId: string;
  date: string /* YYYY-MM-DD */;
  time?: string /* HH:MM (24H) */;
  guests?: string;
}
