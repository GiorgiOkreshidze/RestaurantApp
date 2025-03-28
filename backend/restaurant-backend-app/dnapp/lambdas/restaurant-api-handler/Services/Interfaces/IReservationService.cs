using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Reservations;
using Function.Models.User;
using Function.Models.Requests;

namespace Function.Services.Interfaces;

public interface IReservationService
{
    Task<Reservation> UpsertReservationAsync(ReservationRequest reservationRequest, User user);
    Task<List<Reservation>> GetReservationsAsync(ReservationsQueryParameters queryParameters, string info, Roles role);
    Task CancelReservationAsync(string reservationId);
}