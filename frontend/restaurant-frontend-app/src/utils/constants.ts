export const apiURL = import.meta.env["VITE_API_URL"];
export const apiURLLocal = "/api";

export const serverRoute = {
  // AUTH
  signUp: "auth/signup", // +
  signIn: "auth/signin", // +
  refresh: "auth/refresh", // ???
  signOut: "auth/signout", // +

  // USER
  userProfile: "users/profile", // +

  // DISHES
  popularDishes: "dishes/popular", // +
  dishes: "dishes", // +
  specialityDishes: "speciality-dishes", // +

  // LOCATIONS
  locations: "locations", // +
  selectOptions: "locations/select-options", // +

  // RESERVATION
  tables: "reservations/tables", // +
  reservations: "reservations", // +
  upsertClientReservation: "reservations/client", // +
  upsertWaiterReservation: "reservations/waiter", // ???

  // FEEDBACK
  feedbacks: "feedbacks", // +

  // MOCKED PATHS (LOCAL API)
  locationTables: "location-tables", // +
  timeSlots: "timeslots", // +

  // OTHER
  users: "users",
};