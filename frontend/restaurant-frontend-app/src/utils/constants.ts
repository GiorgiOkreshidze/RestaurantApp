export const apiURL = import.meta.env["VITE_API_URL"];

export const serverRoute = {
  signUp: "signup",
  signIn: "signin",
  dishes: "dishes",
  popularDishes: "dishes/popular",
  locations: "locations",
  specialityDishes: "speciality-dishes",
  signOut: "signout",
  userData: "users/profile",
  reservations: "reservations",
  upsertClientReservation: "reservations/client",
  deleteClientReservation: "reservations/client",
  selectOptions: "locations/select-options",
  tables: "bookings/tables",
};
