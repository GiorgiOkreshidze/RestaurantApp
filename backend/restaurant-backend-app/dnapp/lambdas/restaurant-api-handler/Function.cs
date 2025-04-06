using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Function.Actions;
using Function.ApiHandler;
using Function.Exceptions;
using ResourceNotFoundException = Function.Exceptions.ResourceNotFoundException;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SimpleLambdaFunction;

public class Function
{
    private readonly ApiHandler _apiHandler;
    
    public Function()
    {
        _apiHandler = new ApiHandler();
    }
    
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Incoming serialized request: " + JsonSerializer.Serialize(request));
        
        APIGatewayProxyResponse result;

        try
        {
            result = await _apiHandler.HandleRequest(request, context);
        }
        catch (ArgumentException e)
        {
            result = ActionUtils.FormatResponse(400, new { message = e.Message });
        }
        catch (AuthenticationException e)
        {
            result = ActionUtils.FormatResponse(401, new { message = e.Message });
        }
        catch (UnauthorizedException e)
        {
            result = ActionUtils.FormatResponse(403, new { message = e.Message });
        }
        catch (ResourceNotFoundException e)
        {
            result = ActionUtils.FormatResponse(404, new { message = e.Message });
        }
        catch (ResourceAlreadyExistsException e)
        {
            result = ActionUtils.FormatResponse(409, new { message = e.Message });
        }
        catch (Exception e)
        {
            context.Logger.LogError(e.Message);
            result = ActionUtils.FormatResponse(500, new { message = "Ops... An error occured. Please try-again later." });
        }

        result.Headers = new Dictionary<string, string>
        {
            { "Access-Control-Allow-Origin", "*" },
            { "Access-Control-Allow-Methods", "GET,POST,OPTIONS" },
            { "Access-Control-Allow-Headers", "Content-Type,Authorization,X-Amz-Security-Token,X-Access-Token,x-amz-security-token" }
        };

        return result;
    }
}
