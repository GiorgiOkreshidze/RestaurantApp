using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

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

        var result = await _apiHandler.HandleRequest(request, context);

        // I didn't found any examples/other solutions to add them on configuration.
        result.Headers = new Dictionary<string, string>
        {
            { "Access-Control-Allow-Origin", "*" },
            { "Access-Control-Allow-Methods", "GET,POST,OPTIONS" },
            { "Access-Control-Allow-Headers", "Content-Type,Authorization" }
        };

        return result;
    }
}
