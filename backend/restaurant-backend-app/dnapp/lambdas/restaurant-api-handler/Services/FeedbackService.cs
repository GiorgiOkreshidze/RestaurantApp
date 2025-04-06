using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Function.Models;
using Function.Models.Requests;
using Function.Models.Responses;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Factories;
using Function.Services.Interfaces;

namespace Function.Services;

public class FeedbackService : IFeedbackService
{
    private readonly ILocationFeedbackFactory _locationFeedbackFactory;
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserRepository _userRepository;
    
    public FeedbackService()
    {
        _feedbackRepository = new FeedbackRepository();
        _reservationRepository = new ReservationRepository();
        _userRepository = new UserRepository();
        _locationFeedbackFactory = new LocationFeedbackFactory();
    }
    
    public async Task<(List<LocationFeedback>, string?)> GetLocationFeedbacksAsync(LocationFeedbackQueryParameters queryParameters)
    {
        return await _feedbackRepository.GetLocationFeedbacksAsync(queryParameters);
    }


    
    public async Task AddFeedbackAsync(ReservationFeedbackRequest reservationFeedbackRequest, string userId)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(reservationFeedbackRequest.ReservationId);
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user is null) throw new UnauthorizedException("User is not registered");

        if (reservation.UserEmail != user.Email)
            throw new UnauthorizedException("You are not authorized to add feedback for this reservation");
        
        if (reservation.Status != GetEnumDescription(ReservationStatus.InProgress) && 
            reservation.Status != GetEnumDescription(ReservationStatus.Finished)) 
            throw new ArgumentException("Reservation should be in status 'In Progress' or 'Finished'");

        if (!string.IsNullOrEmpty(reservationFeedbackRequest.CuisineRating) &&
            int.TryParse(reservationFeedbackRequest.CuisineRating, out var cuisineRating))
        {
            if (cuisineRating is < 0 or > 5) throw new ArgumentException("Cuisine rating must be between 0 and 5");
        }

        if (!string.IsNullOrEmpty(reservationFeedbackRequest.ServiceRating) &&
            int.TryParse(reservationFeedbackRequest.ServiceRating, out var serviceRating))
        {
            if (serviceRating is < 0 or > 5) throw new ArgumentException("Service rating must be between 0 and 5");
        }

        var feedbacks = await _locationFeedbackFactory.CreateFeedbacksAsync(reservationFeedbackRequest, user, reservation);
        
        foreach (var feedback in feedbacks)
        {
            await _feedbackRepository.UpsertFeedbackByReservationAndTypeAsync(feedback);
        }
    }

    private static string GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }

    public async Task<int> TotalItemCountAsync()
    {
        return await _feedbackRepository.GetTotalItemCount();
    }
}