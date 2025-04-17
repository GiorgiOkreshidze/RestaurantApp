import { timeslotsHandlers } from "./endpoints/timeslots";
import { locationTables } from "./endpoints/location-tables";

export const handlers = [...timeslotsHandlers, ...locationTables];