using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Function.Actions;
using Function.Actions.Auth;
using Function.Actions.Dishes;
using Function.Actions.Feedbacks;
using Function.Actions.Locations;
using Function.Actions.Reservations;
using Function.Actions.Tables;
using Function.Actions.Users;

namespace Function.ApiHandler;

public class ApiHandler
{
    //Actions
    private readonly SignUpAction _signupAction;
    private readonly SignInAction _signInAction;
    private readonly SignOutAction _signOutAction;
    private readonly GetLocationsAction _getLocationsActions;
    private readonly GetPopularDishesAction _getPopularDishesAction;
    private readonly RefreshTokenAction _refreshTokenAction;
    private readonly GetProfileAction _getProfileAction;
    private readonly GetSpecialityDishesAction _getSpecialityDishesAction;
    private readonly GetLocationOptionsAction _getLocationOptionsAction;
    private readonly GetLocationFeedbacksAction _getLocationFeedbacksAction;
    private readonly CreateClientReservationAction _createClientReservationAction;
    private readonly GetAvailableTablesAction _getAvailableTablesAction;
    private readonly GetReservationsAction _getReservationsAction;
    private readonly DeleteReservationAction _deleteReservationAction;
    private readonly GetAllCustomersAction _getAllCustomersAction;
    private readonly GetAllDishesAction _getAllDishesAction;
    private readonly GetDishByIdAction _getDishByIdAction;
    private readonly CreateWaiterReservationAction _createWaiterReservationAction;

    public ApiHandler()
    {
        // Init Actions
        _signupAction = new SignUpAction();
        _signInAction = new SignInAction();
        _signOutAction = new SignOutAction();
        _getLocationsActions = new GetLocationsAction();
        _getPopularDishesAction = new GetPopularDishesAction();
        _refreshTokenAction = new RefreshTokenAction();
        _getProfileAction = new GetProfileAction();
        _getSpecialityDishesAction = new GetSpecialityDishesAction();
        _getLocationOptionsAction = new GetLocationOptionsAction();
        _createClientReservationAction = new CreateClientReservationAction();
        _getLocationFeedbacksAction = new GetLocationFeedbacksAction();
        _getAvailableTablesAction = new GetAvailableTablesAction();
        _getReservationsAction = new GetReservationsAction();
        _deleteReservationAction = new DeleteReservationAction();
        _getAllCustomersAction = new GetAllCustomersAction();
        _getAllDishesAction = new GetAllDishesAction();
        _getDishByIdAction = new GetDishByIdAction();
        _createWaiterReservationAction = new CreateWaiterReservationAction();
    }

    public async Task<APIGatewayProxyResponse> HandleRequest(APIGatewayProxyRequest eventRequest,
        ILambdaContext context)
    {
        var requestPath = eventRequest.Resource;
        var methodName = eventRequest.HttpMethod;

        context.Logger.LogInformation("eventRequest.Resource: " + requestPath);
        context.Logger.LogInformation("eventRequest.HttpMethod: " + methodName);

        // Add your new endpoints here
        var actionEndpointMapping =
            new Dictionary<string,
                Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>>()
            {
                {
                    "/signup", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "POST", _signupAction.Signup }
                    }
                },
                {
                    "/signin", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "POST", _signInAction.Signin }
                    }
                },
                {
                    "/signout", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "POST", _signOutAction.SignOut }
                    }
                },
                {
                    "/auth/refresh", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "POST", _refreshTokenAction.RefreshToken }
                    }
                },
                {
                    "/locations", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getLocationsActions.GetLocationsAsync }
                    }
                },
                {
                    "/dishes/popular", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getPopularDishesAction.GetPopularDishesAsync }
                    }
                },
                {
                    "/dishes", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getAllDishesAction.GetAllDishesAsync }
                    }
                },
                {
                    "/dishes/{id}", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getDishByIdAction.GetDishByIdAsync }
                    }
                },
                {
                    "/users/profile", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getProfileAction.GetProfile }
                    }
                },
                {
                    "/locations/{id}/speciality-dishes", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getSpecialityDishesAction.GetSpecialityDishesAsync }
                    }
                },
                {
                    "/locations/select-options", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getLocationOptionsAction.GetOptionsAsync }
                    }
                },
                {
                    "/reservations/client", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "POST", _createClientReservationAction.CreateReservationAsync }
                    }
                },
                {
                    "/reservations/waiter", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "POST", _createWaiterReservationAction.CreateReservationAsync }
                    }
                },
                {
                    "/locations/{id}/feedbacks", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getLocationFeedbacksAction.GetLocationFeedbacks }
                    }
                },
                {
                    "/bookings/tables", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getAvailableTablesAction.GetAvailableTables }
                    }
                },
                {
                    "/reservations", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getReservationsAction.GetReservationsAsync }
                    }
                },
                {
                    "/reservations/{id}", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "DELETE", _deleteReservationAction.DeleteReservationAsync }
                    }
                },
                {
                    "/users", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getAllCustomersAction.GetAllCustomersAsync }
                    }
                },
            };

        if (!actionEndpointMapping.TryGetValue(requestPath, out var resourceMethods) ||
            !resourceMethods.TryGetValue(methodName, out var action))
        {
            return ActionUtils.InvalidEndpoint(requestPath, methodName);
        }

        if (!string.IsNullOrEmpty(eventRequest.Body))
        {
            eventRequest.Body = eventRequest.Body.Trim();
        }

        return await action(eventRequest);
    }
}