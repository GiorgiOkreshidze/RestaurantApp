using System;
using System.Collections.Generic;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace SimpleLambdaFunction.Actions;

public class ActionUtils
{
    public static APIGatewayProxyResponse FormatResponse(int code, object response)
    {
        var responseString = JsonSerializer.Serialize(response);
        Console.WriteLine("JsonSerializer.Serialize(response): " + responseString);

        return new APIGatewayProxyResponse
        {
            StatusCode = code,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
            Body = responseString
        };
    }
    
    public static List<string> ValidateRequestParams(string[] expected, Dictionary<string, JsonElement> received)
    {
        var missing = new List<string>();
        foreach (var param in expected)
        {
            if (!received.ContainsKey(param))
            {
                missing.Add(param);
            }
        }

        return missing;
    }
    
    public static APIGatewayProxyResponse InvalidEndpoint(string path, string method)
    {
        return FormatResponse(400,
            new
            {
                message = $"Bad request syntax or unsupported method. Request path: {path}. HTTP method: {method}"
            });
    }
}