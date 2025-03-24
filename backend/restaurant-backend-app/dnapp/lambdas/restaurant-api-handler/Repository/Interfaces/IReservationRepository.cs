using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;

namespace Function.Repository.Interfaces;

public interface IReservationRepository
{
    Task<Reservation> UpsertReservation(Reservation reservation);
    Task<List<Reservation>> GetReservationsByDateLocationTable(string date, string locationAddress, string tableNumber);
    Task<List<ReservationInfo>> GetReservationsForDateAndLocation(string date, string locationAddress);
    Task<List<Reservation>> GetCustomerReservationsAsync(string info);
    Task CancelReservation(string reservationId);
}