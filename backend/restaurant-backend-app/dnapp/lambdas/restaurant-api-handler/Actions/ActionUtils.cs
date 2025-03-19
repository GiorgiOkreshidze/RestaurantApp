using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Amazon.CognitoIdentityProvider.Model;
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

    public static bool ValidateName(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Length > 50)
            return false;

        string namePattern = @"^[a-zA-Z\-']+$";
        return Regex.IsMatch(name, namePattern);
    }

    public static bool ValidateEmail(string email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    public static bool ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8 || password.Length > 16)
            return false;

        return Regex.IsMatch(password, @"(?=.*[A-Z])") &&
               Regex.IsMatch(password, @"(?=.*[a-z])") &&
               Regex.IsMatch(password, @"(?=.*\d)") &&
               Regex.IsMatch(password, @"(?=.*[!@#$%^&*()_+\-=[\]{};':""\\|,.<>/?])");
    }

    public static string GetAccessToken(APIGatewayProxyRequest request)
    {
        if (!request.Headers.TryGetValue("X-Access-Token", out var accessTokenHeader) ||
            string.IsNullOrEmpty(accessTokenHeader))
        {
            throw new UnauthorizedException("Access token is missing");
        }

        return accessTokenHeader.Trim();
    }
}