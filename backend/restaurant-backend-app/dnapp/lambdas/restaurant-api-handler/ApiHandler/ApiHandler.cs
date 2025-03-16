using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Function.Actions;
using SimpleLambdaFunction.Actions;

namespace SimpleLambdaFunction;

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
    private readonly GetLocationOptions _getLocationOptions;


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
        _getLocationOptions = new GetLocationOptions();
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
                        { "POST", _signOutAction.Signout }
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
                        { "GET", _getLocationsActions.GetLocations }
                    }
                },
                {
                    "/dishes/popular", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getPopularDishesAction.GetPopularDishes }
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
                        { "GET", _getSpecialityDishesAction.GetSpecialityDishes }
                    }
                },
                      {
                    "/locations/select-options", new Dictionary<string, Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>>>
                    {
                        { "GET", _getLocationOptions.GetOptions }
                    }
                }
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