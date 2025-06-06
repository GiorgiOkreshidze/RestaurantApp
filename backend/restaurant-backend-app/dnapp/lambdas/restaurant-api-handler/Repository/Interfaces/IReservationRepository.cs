﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Reservations;

namespace Function.Repository.Interfaces;

public interface IReservationRepository
{
    Task<Reservation> UpsertReservationAsync(Reservation reservation);

    Task<List<Reservation>> GetReservationsByDateLocationTable(string date, string locationAddress, string tableNumber);

    Task<List<ReservationInfo>> GetReservationsForDateAndLocation(string date, string locationId);

    Task<List<Reservation>> GetCustomerReservationsAsync(string email);

    Task<List<Reservation>> GetWaiterReservationsAsync(ReservationsQueryParameters queryParams, string userId);

    Task CancelReservation(string reservationId);

    Task<int> GetWaiterReservationCountAsync(string waiterId, string date);

    Task<bool> ReservationExistsAsync(string reservationId);

    Task<Reservation> GetReservationByIdAsync(string reservationId);
}