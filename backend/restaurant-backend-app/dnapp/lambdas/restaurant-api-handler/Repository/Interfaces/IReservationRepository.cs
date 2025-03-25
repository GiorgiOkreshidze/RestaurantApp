using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Reservations;

namespace Function.Repository.Interfaces;

public interface IReservationRepository
{
    Task<Reservation> UpsertReservation(Reservation reservation);
    Task<List<Reservation>> GetReservationsByDateLocationTable(string date, string locationAddress, string tableNumber);
    Task<List<ReservationInfo>> GetReservationsForDateAndLocation(string date, string locationId);
    Task<List<Reservation>> GetCustomerReservationsAsync(ReservationsQueryParameters queryParams, string info);
    Task<List<Reservation>> GetWaiterReservationsAsync(ReservationsQueryParameters queryParams, string info);
    Task CancelReservation(string reservationId);
}