using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.Requests;
using Function.Models.Reservations;
using Function.Models.Responses;
using Function.Models.User;
using Function.Services.Interfaces;

namespace Function.Services.Factories;

public class LocationFeedbackFactory : ILocationFeedbackFactory
{
    public async Task<LocationFeedback[]> CreateFeedbacksAsync(ReservationFeedbackRequest request, User user, Reservation reservation)
    {
        var feedbacks = new List<LocationFeedback>();
        var currentDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        if (!string.IsNullOrEmpty(request.CuisineRating))
        {
            feedbacks.Add(new LocationFeedback
            {
                Id = Guid.NewGuid().ToString(),
                Rate = request.CuisineRating,
                Comment = request.CuisineComment ?? string.Empty,
                UserName = user.GetFullName(),
                UserAvatarUrl = user.ImageUrl,
                Date = currentDate,
                Type = "CUISINE_EXPERIENCE",
                LocationId = reservation.LocationId,
                ReservationId = reservation.Id
            });
        }

        if (!string.IsNullOrEmpty(request.ServiceRating))
        {
            feedbacks.Add(new LocationFeedback
            {
                Id = Guid.NewGuid().ToString(),
                Rate = request.ServiceRating,
                Comment = request.ServiceComment ?? string.Empty,
                UserName = user.GetFullName(),
                UserAvatarUrl = user.ImageUrl,
                Date = currentDate,
                Type = "SERVICE_QUALITY",
                LocationId = reservation.LocationId,
                ReservationId = reservation.Id
            });
        }

        return feedbacks.ToArray();
    }
}