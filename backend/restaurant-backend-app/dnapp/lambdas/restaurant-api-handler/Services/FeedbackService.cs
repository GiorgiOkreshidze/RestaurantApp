using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Function.Models;
using Function.Models.Feedbacks;
using Function.Models.Requests;
using Function.Models.Responses;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;

namespace Function.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserRepository _userRepository;
    
    public FeedbackService()
    {
        _feedbackRepository = new FeedbackRepository();
        _reservationRepository = new ReservationRepository();
        _userRepository = new UserRepository();
    }
    
    public async Task<(List<LocationFeedbackResponse>, string?)> GetLocationFeedbacksAsync(LocationFeedbackQueryParameters queryParameters)
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

        if (int.TryParse(reservationFeedbackRequest.CuisineRating, out var cuisineRating) &&
            int.TryParse(reservationFeedbackRequest.ServiceRating, out var serviceRating))
        {
            if (cuisineRating is < 0 or > 5) throw new ArgumentException("Cuisine rating must be between 0 and 5");
            
            if (serviceRating is < 0 or > 5) throw new ArgumentException("Service rating must be between 0 and 5");
        }
        else
        {
            throw new ArgumentException("Cuisine and Service rating must be numbers and between 0 and 5");
        }


        var feedback = new ReservationFeedback
        {
            ReservationId = reservationFeedbackRequest.ReservationId,
            CuisineComment = reservationFeedbackRequest.CuisineComment,
            ServiceComment = reservationFeedbackRequest.ServiceComment,
            CuisineRating = reservationFeedbackRequest.CuisineRating,
            ServiceRating = reservationFeedbackRequest.ServiceRating,
        };
        
        await _feedbackRepository.UpsertReservationFeedbackAsync(feedback);
    }

    private static string GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
}