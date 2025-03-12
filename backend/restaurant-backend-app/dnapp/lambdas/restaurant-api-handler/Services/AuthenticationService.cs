using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using SimpleLambdaFunction.Services.Interfaces;
using Amazon.Lambda.Core;
using Amazon.CognitoIdentityProvider.Model;
using System.Collections.Generic;
using System;
using Function.Exceptions;
using System.Security.Authentication;

namespace SimpleLambdaFunction.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
    private readonly string? _clientId = Environment.GetEnvironmentVariable("cup_client_id");
    private readonly string? _userPoolId = Environment.GetEnvironmentVariable("cup_id");

    public AuthenticationService()
    {
        _cognitoClient = new AmazonCognitoIdentityProviderClient();
    }

    public async Task<string> SignIn(string email, string password)
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
            return authResponse.AuthenticationResult.IdToken;
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

    public async Task SignUp(string firstName, string lastName, string email, string password)
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
    }

    public async Task SignOut(string accessToken)
    {
        try
        {
            var signOutRequest = new GlobalSignOutRequest
            {
                AccessToken = accessToken
            };
            await _cognitoClient.GlobalSignOutAsync(signOutRequest);
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
}