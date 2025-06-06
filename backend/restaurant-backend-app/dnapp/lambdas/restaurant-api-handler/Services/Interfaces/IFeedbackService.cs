﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Requests;
using Function.Models.Responses;

namespace Function.Services.Interfaces;

public interface IFeedbackService
{
    Task<(List<LocationFeedback>, string?)> GetLocationFeedbacksAsync(LocationFeedbackQueryParameters queryParameters);
    
    Task AddFeedbackAsync(ReservationFeedbackRequest reservationFeedbackRequest, string userId);

    Task<int> TotalItemCountAsync();
}