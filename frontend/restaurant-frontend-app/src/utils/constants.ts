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
  locationTables: "location-tables",
};

export const USER_ROLE = {
  CUSTOMER: "Customer",
  WAITER: "Waiter",
};