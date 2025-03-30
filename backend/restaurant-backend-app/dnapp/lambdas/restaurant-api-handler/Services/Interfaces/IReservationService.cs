using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.Interfaces;
using Function.Models.Reservations;
using Function.Models.User;

namespace Function.Services.Interfaces;

public interface IReservationService
{
    Task<Reservation> UpsertReservationAsync(IReservationRequest reservationRequest, User user);
    Task<List<Reservation>> GetReservationsAsync(ReservationsQueryParameters queryParameters, string info, Roles role);
    Task CancelReservationAsync(string reservationId);
}