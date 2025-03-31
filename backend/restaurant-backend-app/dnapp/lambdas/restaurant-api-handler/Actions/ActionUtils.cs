using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.RegularExpressions;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.Reservations;

namespace Function.Actions;

public static class ActionUtils
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
    
    public static void ValidateRequiredParams(string[] requiredParams, Dictionary<string, JsonElement> body)
    {
        var missingParams = ValidateRequestParams(requiredParams, body);

        if (missingParams.Count > 0)
        {
            throw new ArgumentException($"Missing required parameters: {string.Join(", ", missingParams)}");
        }
    }
    
    public static void ValidateRequestBody(string requestBody)
    {
        if (string.IsNullOrEmpty(requestBody))
        {
            throw new ArgumentException("Request body is missing");
        }
    }
    
    public static APIGatewayProxyResponse InvalidEndpoint(string path, string method)
    {
        throw new ArgumentException( $"Bad request syntax or unsupported method. Request path: {path}. HTTP method: {method}");
    }

    public static void ValidateFullName(string? name)
    {
        const string namePattern = @"^[a-zA-Z\-']+$";

        if (string.IsNullOrEmpty(name) || name.Length > 50 || !Regex.IsMatch(name, namePattern))
            throw new ArgumentException(
                "First name and last name must be up to 50 characters. Only Latin letters, hyphens, and apostrophes are allowed.");
    }

    public static void ValidateEmail(string? email)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, emailPattern))
        {
            throw new ArgumentException(
                "Email must be in following format test@email.com");
        }
    }

    public static void ValidatePassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8 || password.Length > 16 ||
            !Regex.IsMatch(password, @"(?=.*[A-Z])") ||
            !Regex.IsMatch(password, @"(?=.*[a-z])") ||
            !Regex.IsMatch(password, @"(?=.*\d)") ||
            !Regex.IsMatch(password, @"(?=.*[!@#$%^&*()_+\-=[\]{};':""\\|,.<>/?])"))
        {
            throw new ArgumentException(
                "Password must be 8-16 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }
    }


    public static JwtSecurityToken ExtractJwtToken(APIGatewayProxyRequest request)
    {
        if (!request.Headers.TryGetValue("Authorization", out var idToken) || string.IsNullOrEmpty(idToken) || !idToken.StartsWith("Bearer "))
        {
            throw new ArgumentException("Authorization header empty or invalid.");
        }

        var token = idToken.Substring("Bearer ".Length).Trim();
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(token))
        {
            throw new ArgumentException("Invalid token format.");
        }

        return tokenHandler.ReadJwtToken(token);
    }


    public static List<TimeSlot> GeneratePredefinedTimeSlots()
    {
        var slots = new List<TimeSlot>();
        var startTimeUtc = new TimeSpan(6, 30, 0); // 6:30 AM UTC (10:30 AM Tbilisi time)
        var endTimeUtc = new TimeSpan(18, 30, 0); // 6:30 PM UTC (10:30 PM Tbilisi time)
        var currentTime = startTimeUtc;

        while (currentTime <= endTimeUtc)
        {
            var slotEnd = currentTime.Add(TimeSpan.FromMinutes(90));

            slots.Add(new TimeSlot
            {
                Start = currentTime.ToString(@"hh\:mm"),
                End = slotEnd.ToString(@"hh\:mm")
            });

            currentTime = currentTime.Add(TimeSpan.FromMinutes(90 + 15)); // 90-minute slot + 15-minute gap
        }

        return slots;
    }

    private static List<string> ValidateRequestParams(string[] expected, Dictionary<string, JsonElement> received)
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
}