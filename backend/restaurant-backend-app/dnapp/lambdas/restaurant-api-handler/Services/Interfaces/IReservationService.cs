using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Requests;

namespace Function.Services.Interfaces;

public interface IReservationService
{
    Task<Reservation> UpsertReservationAsync(ReservationRequest reservationRequest, string fullName);
    Task<List<Reservation>> GetCustomerReservationsAsync(string info);
    Task CancelReservationAsync(string reservationId);
}