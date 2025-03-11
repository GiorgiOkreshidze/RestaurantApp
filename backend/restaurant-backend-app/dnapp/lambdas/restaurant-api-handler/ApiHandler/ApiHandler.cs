using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using SimpleLambdaFunction.Actions;

namespace SimpleLambdaFunction;

public class ApiHandler
{
    //Actions
    private readonly SignUpAction _signupAction;
    private readonly SignInAction _signInAction;
    
    public ApiHandler()
    {
        // Init Actions
        _signupAction = new SignUpAction();
        _signInAction = new SignInAction();
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