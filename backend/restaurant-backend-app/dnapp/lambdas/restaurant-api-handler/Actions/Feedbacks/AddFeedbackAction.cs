using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.Requests;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Feedbacks;

public class AddFeedbackAction
{
    private readonly IFeedbackService _feedbackService;
    
    public AddFeedbackAction()
    {
        _feedbackService = new FeedbackService();
    }
    
    public async Task<APIGatewayProxyResponse> AddFeedbackAsync(APIGatewayProxyRequest request)
    {
        var jwtToken = ActionUtils.ExtractJwtToken(request);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var feedback = JsonSerializer.Deserialize<ReservationFeedbackRequest>(request.Body);

        if (string.IsNullOrEmpty(userId)) throw new UnauthorizedException("User is not registered");

        if (feedback is null) throw new ArgumentException("Feedback request body was null");
        
        await _feedbackService.AddFeedbackAsync(feedback, userId);
        
        return ActionUtils.FormatResponse(201, new { message = "Feedback added successfully" });
    }
}