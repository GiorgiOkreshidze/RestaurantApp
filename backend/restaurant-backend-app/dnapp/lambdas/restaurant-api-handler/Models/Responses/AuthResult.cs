﻿using System.Text.Json.Serialization;

namespace Function.Models.Responses;

public class AuthResult
{
    [JsonPropertyName("idToken")]
    public required string IdToken { get; set; }
        
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }
}