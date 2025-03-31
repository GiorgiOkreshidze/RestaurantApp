using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Function.Models;
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
    
    public FeedbackService()
    {
        _feedbackRepository = new FeedbackRepository();
        _reservationRepository = new ReservationRepository();
    }
    
    public async Task<(List<LocationFeedbackResponse>, string?)> GetLocationFeedbacksAsync(LocationFeedbackQueryParameters queryParameters)
    {
        return await _feedbackRepository.GetLocationFeedbacksAsync(queryParameters);
    }
    
    public async Task AddFeedbackAsync(FeedbackRequest feedbackRequest)
    {
        //get reservationId from db
        var reservation = await _reservationRepository.GetReservationByIdAsync(feedbackRequest.ReservationId);
        
        // check if reservation belongs to user
        if (reservation.UserEmail != fetchCurrentUserFromToken().userEmail)
        {
            throw new UnauthorizedException("You are not authorized to add feedback for this reservation");
        }
        

        
        // check if reservation is in status "In Progress" or "Completed" / otherwise throw argument exception
        if (reservation.Status != ReservationStatus.InProgress && reservation.Status != ReservationStatus.Completed)
        {
            throw new ArgumentException("Reservation is not in progress or completed");
        }
        
        // check that rating can not be negative and no more then 5
        if (int.TryParse(feedbackRequest.CuisineRating, out var cuisineRating) && int.TryParse(feedbackRequest.ServiceRating, out var serviceRating))
        {
            if (cuisineRating < 0 || cuisineRating > 5)
            {
                throw new ArgumentException("Cuisine rating must be between 0 and 5");
            }
            if (serviceRating < 0 || serviceRating > 5)
            {
                throw new ArgumentException("Service rating must be between 0 and 5");
            }
        }
        else
        {
            throw new ArgumentException("Cuisine rating and Service rating must be numbers");
        }
        
        // Check if reservation alredy has feedback if yes update it
        var feedback = await _feedbackRepository.GetFeedbackByReservationIdAsync(feedbackRequest.ReservationId);
        if (feedback != null)
        {
            await _feedbackRepository.UpdateFeedbackAsync(feedbackRequest, feedback);
        }
        else
        {
            await _feedbackRepository.AddFeedbackAsync(feedbackRequest);
        }
    }
}