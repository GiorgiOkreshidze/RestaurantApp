using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using SimpleLambdaFunction.Services.Interfaces;
using Amazon.CognitoIdentityProvider.Model;
using System.Collections.Generic;
using System;
using Function.Exceptions;
using System.Security.Authentication;
using Function.Models;

namespace SimpleLambdaFunction.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
    private readonly string? _clientId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_CLIENT_ID");
    private readonly string? _userPoolId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_ID");

    public AuthenticationService()
    {
        _cognitoClient = new AmazonCognitoIdentityProviderClient();
    }

    public async Task<AuthResult> SignIn(string email, string password)
    {
        var authRequest = new AdminInitiateAuthRequest
        {
            AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH,
            ClientId = _clientId,
            UserPoolId = _userPoolId,
            AuthParameters = new Dictionary<string, string>
            {
                { "USERNAME", email },
                { "PASSWORD", password }
            }
        };

        try
        {
            var authResponse = await _cognitoClient.AdminInitiateAuthAsync(authRequest);
            
            var response = new AuthResult
            {
                IdToken = authResponse.AuthenticationResult.IdToken,
                RefreshToken = authResponse.AuthenticationResult.RefreshToken,
                ExpiresIn = authResponse.AuthenticationResult.ExpiresIn
            };

            return response;
        }
        catch (UserNotFoundException)
        {
            throw new AuthenticationException("We could not find an account matching the email.");
        }
        catch (NotAuthorizedException)
        {
            throw new AuthenticationException("Incorrect email or password. Try again or create an account.");
        }
        catch (TooManyRequestsException)
        {
            throw new AuthenticationException("Your account is temporarily locked due to multiple failed login attempts. Please try again later.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log in: {ex}");
            throw;
        }
    }

    public async Task SignUp(string firstName, string lastName, string email, string password, Roles role = Roles.Customer)
    {
        var signUpRequest = new SignUpRequest
        {
            ClientId = _clientId,
            Username = email,
            Password = password,
            UserAttributes = new List<AttributeType>
            {
                new AttributeType { Name = "given_name", Value = firstName },
                new AttributeType { Name = "family_name", Value = lastName },
                new AttributeType { Name = "email", Value = email }
            }
        };

        await _cognitoClient.SignUpAsync(signUpRequest);

        var confirmRequest = new AdminConfirmSignUpRequest
        {
            UserPoolId = _userPoolId,
            Username = email
        };

        await _cognitoClient.AdminConfirmSignUpAsync(confirmRequest);

        var updateAttributesRequest = new AdminUpdateUserAttributesRequest
        {
            UserPoolId = _userPoolId,
            Username = email,
            UserAttributes = new List<AttributeType>
            {
                new AttributeType { Name = "custom:role", Value = role.ToString() }
            }
        };
        await _cognitoClient.AdminUpdateUserAttributesAsync(updateAttributesRequest);
    }

    public async Task SignOut(string refreshToken)
    {
        try
        {
            var revokeRequest = new RevokeTokenRequest
            {
                Token = refreshToken,
                ClientId = _clientId
            };

            await _cognitoClient.RevokeTokenAsync(revokeRequest);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log out: {ex}");
            throw new AuthenticationException("Logout failed. Please try again.");
        }
    }

    public async Task CheckEmailUniqueness(string email)
    {
        try
        {
            // Try to find existing user by email
            var listUsersRequest = new ListUsersRequest
            {
                UserPoolId = _userPoolId,
                Filter = $"email = \"{email}\""
            };

            var response = await _cognitoClient.ListUsersAsync(listUsersRequest);

            if (response.Users.Count > 0)
            {
                throw new UserExistsException($"User with email {email} already exists");
            }
        }
        catch (UserExistsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking email uniqueness: {ex.Message}");
        }
    }

    public async Task<AuthResult> RefreshToken(string refreshToken)
    {
        var authRequest = new AdminInitiateAuthRequest
        {
            AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
            ClientId = _clientId,
            UserPoolId = _userPoolId,
            AuthParameters = new Dictionary<string, string>
            {
                { "REFRESH_TOKEN", refreshToken }
            }
        };

        try
        {
            var response = await _cognitoClient.AdminInitiateAuthAsync(authRequest);

            var authResult = new AuthResult
            {
                IdToken = response.AuthenticationResult.IdToken,
                RefreshToken = response.AuthenticationResult.RefreshToken ?? refreshToken,
                ExpiresIn = response.AuthenticationResult.ExpiresIn
            };

            return authResult;
        }
        catch (NotAuthorizedException ex)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to refresh token in: {ex}");
            throw;
        }
    }
}