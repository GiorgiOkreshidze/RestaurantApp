using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Function.Actions;
using Function.Models;
using Function.Models.Requests;
using Function.Models.Reservations;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Repository;

namespace Function.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly ILocationRepository _locationRepository;

    public TableService()
    {
        _reservationRepository = new ReservationRepository();
        _tableRepository = new TableRepository();
        _locationRepository = new LocationRepository();
    }

    public async Task<List<TableResponse>> GetAvailableTablesAsync(string locationId, string date, string? time, int guests)
    {
        var location = await _locationRepository.GetLocationByIdAsync(locationId);
        var tables = await _tableRepository.GetTablesForLocationAsync(location.Id, guests);
        var reservations = await _reservationRepository.GetReservationsForDateAndLocation(date, location.Id);
        var result = CalculateAvailableSlots(tables, reservations, time);
        var tablesWithSlots = result.Where(t => t.AvailableSlots.Count != 0).ToList();
        return tablesWithSlots;
    }

    private List<TableResponse> CalculateAvailableSlots(
        List<RestaurantTable> tables,
        List<ReservationInfo> reservations,
        string? requestedTime)
    {
        var result = new List<TableResponse>();

        foreach (var table in tables)
        {
            var allTimeSlots = ActionUtils.GeneratePredefinedTimeSlots();
            var tableReservations = reservations
                .Where(r => r.TableId == table.Id)
                .ToList();
            var availableSlots = FilterAvailableTimeSlots(allTimeSlots, tableReservations);

            if (!string.IsNullOrEmpty(requestedTime))
            {
                availableSlots = FilterSlotsByRequestedTime(availableSlots, requestedTime);

                if (!availableSlots.Any())
                {
                    continue;
                }
            }

            result.Add(new TableResponse
            {
                TableId = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                LocationId = table.LocationId,
                LocationAddress = table.LocationAddress,
                AvailableSlots = availableSlots
            });
        }

        return result;
    }

    private List<TimeSlot> FilterAvailableTimeSlots(List<TimeSlot> allSlots, List<ReservationInfo> reservations)
    {
        var availableSlots = new List<TimeSlot>();

        foreach (var slot in allSlots)
        {
            var slotStartTime = TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture);
            var slotEndTime = TimeSpan.ParseExact(slot.End, "hh\\:mm", CultureInfo.InvariantCulture);

            var isAvailable = true;

            foreach (var reservation in reservations)
            {
                var reservationStart =
                    TimeSpan.ParseExact(reservation.TimeFrom, "hh\\:mm", CultureInfo.InvariantCulture);
                var reservationEnd = TimeSpan.ParseExact(reservation.TimeTo, "hh\\:mm", CultureInfo.InvariantCulture);

                if (slotStartTime >= reservationEnd || slotEndTime <= reservationStart) continue;
                isAvailable = false;
                break;
            }

            if (isAvailable)
            {
                availableSlots.Add(slot);
            }
        }

        return availableSlots;
    }


    private List<TimeSlot> FilterSlotsByRequestedTime(List<TimeSlot> availableSlots, string requestedTime)
    {
        var requestedTimeSpan = TimeSpan.ParseExact(requestedTime, "hh\\:mm", CultureInfo.InvariantCulture);

        foreach (var slot in availableSlots)
        {
            var slotTime = TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture);
            var slotEndTime = TimeSpan.ParseExact(slot.End, "hh\\:mm", CultureInfo.InvariantCulture);

            // If the requested time falls within this slot's duration
            if (requestedTimeSpan >= slotTime && requestedTimeSpan <= slotEndTime)
            {
                // Return only this slot
                return [slot];
            }
        }

        // If no exact match is found, check for the nearest slot within a 15-minute window
        var nearestSlot = availableSlots
            .Where(slot =>
            {
                var slotTime = TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture);
                var timeDifference = Math.Abs((slotTime - requestedTimeSpan).TotalMinutes);
                return timeDifference <= 15; // Check if the slot is within 15 minutes of the requested time
            }).MinBy(slot =>
                Math.Abs((TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture) - requestedTimeSpan)
                    .Ticks));

        // If the nearest slot is found, return it, // If no slot contains the requested time, return empty list
        return nearestSlot != null ? [nearestSlot] : [];
    }
}