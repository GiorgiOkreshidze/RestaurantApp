using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.Requests.Base;
using Function.Models.Reservations;
using Function.Models.User;

namespace Function.Services.Interfaces;

public interface IReservationService
{
    Task<Reservation> UpsertReservationAsync(BaseReservationRequest reservationRequest, string userId);

    Task<List<Reservation>> GetReservationsAsync(ReservationsQueryParameters queryParameters, string userId, string email, Roles role);

    Task CancelReservationAsync(string reservationId);
}