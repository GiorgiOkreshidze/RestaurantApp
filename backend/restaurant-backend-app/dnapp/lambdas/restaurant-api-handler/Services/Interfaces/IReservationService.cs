using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Requests;
using Function.Models.User;

namespace Function.Services.Interfaces;

public interface IReservationService
{
    Task<Reservation> UpsertReservationAsync(ReservationRequest reservation, User user);
    Task<List<Reservation>> GetCustomerReservationsAsync(string info);
    Task CancelReservationAsync(string reservationId);
}