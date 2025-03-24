using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;

namespace Function.Services.Interfaces;

public interface IReservationService
{
    Task<Reservation> UpsertReservationAsync(Reservation reservation);
    Task<List<Reservation>> GetReservationsByDateLocationTable(string date, string locationAddress, string tableNumber);
    Task<List<ReservationInfo>> GetReservationsForDateAndLocation(string date, string locationAddress);
    Task<List<Reservation>> GetCustomerReservationsAsync(string info);
    Task CancelReservationAsync(string reservationId);
}