import { LocationTable } from "@/types";

export const apiURL = import.meta.env["VITE_API_URL"];

export const serverRoute = {
  signUp: "signup",
  signIn: "signin",
  dishes: "dishes",
  popularDishes: "dishes/popular",
  feedbacks: "feedbacks",
  locations: "locations",
  location: "location",
  specialityDishes: "speciality-dishes",
  signOut: "signout",
  userData: "users/profile",
  reservations: "reservations",
  upsertClientReservation: "reservations/client",
  deleteClientReservation: "reservations",
  selectOptions: "locations/select-options",
  tables: "bookings/tables",
  users: "users",
  upsertWaiterReservation: "reservations/waiter",
  giveReservationFeedback: "feedbacks",
  timeSlots: "timeslots",
};

export const USER_ROLE = {
  CUSTOMER: "Customer",
  WAITER: "Waiter",
};

export const LOCATION_TABLES: LocationTable[] = [
  {
    tableId: "04ba5b37-8fbd-4f5f-8354-0b75078a790a",
    tableNumber: "1",
    capacity: "4",
    locationId: "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
    locationAddress: "48 Rustaveli Avenue",
  },
  {
    tableId: "dc80f954-df0c-457d-a938-26671f0dad47",
    tableNumber: "2",
    capacity: "6",
    locationId: "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
    locationAddress: "48 Rustaveli Avenue",
  },
  {
    tableId: "075312b0-6267-4dc6-ae96-28597231964e",
    tableNumber: "3",
    capacity: "2",
    locationId: "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
    locationAddress: "48 Rustaveli Avenue",
  },
  {
    tableId: "84405cfd-c7f5-4f67-9eef-496485065b1f",
    tableNumber: "4",
    capacity: "10",
    locationId: "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
    locationAddress: "48 Rustaveli Avenue",
  },
  {
    tableId: "c25f24b6-f305-4cd7-a6b6-54f126794c78",
    tableNumber: "1",
    capacity: "2",
    locationId: "e1fcb3b4-bf68-4bcb-b9ba-eac917dafac7",
    locationAddress: "14 Baratashvili Street",
  },
  {
    tableId: "9f9e2205-dc13-438b-802e-5824ad1889eb",
    tableNumber: "2",
    capacity: "4",
    locationId: "e1fcb3b4-bf68-4bcb-b9ba-eac917dafac7",
    locationAddress: "14 Baratashvili Street",
  },
  {
    tableId: "07095e7b-5cc9-40d2-a1d8-65aecdf5a2d9",
    tableNumber: "3",
    capacity: "6",
    locationId: "e1fcb3b4-bf68-4bcb-b9ba-eac917dafac7",
    locationAddress: "14 Baratashvili Street",
  },
  {
    tableId: "b8801e52-ba42-463e-b5cd-68c4604e223e",
    tableNumber: "4",
    capacity: "10",
    locationId: "e1fcb3b4-bf68-4bcb-b9ba-eac917dafac7",
    locationAddress: "14 Baratashvili Street",
  },
  {
    tableId: "42f7291d-0997-4e74-8740-880533094881",
    tableNumber: "1",
    capacity: "6",
    locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
    locationAddress: "9 Abashidze Street",
  },
  {
    tableId: "d7561080-cc9b-497d-ad38-88f264efbbfc",
    tableNumber: "2",
    capacity: "4",
    locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
    locationAddress: "9 Abashidze Street",
  },
  {
    tableId: "146bc291-2926-4a0b-90a2-a54975d2cbba",
    tableNumber: "3",
    capacity: "10",
    locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
    locationAddress: "9 Abashidze Street",
  },
  {
    tableId: "604acc18-d0f6-4d0d-af5c-b14a9aeaa559",
    tableNumber: "4",
    capacity: "10",
    locationId: "3a88c365-970b-4a7a-a206-bc5282b9b25f",
    locationAddress: "9 Abashidze Street",
  },
];
