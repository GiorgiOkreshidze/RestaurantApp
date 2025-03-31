using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Function.Exceptions;
using Function.Models.Requests;
using Function.Models.Responses;
using Function.Models.User;
using Function.Services.Interfaces;
using ResourceNotFoundException = Function.Exceptions.ResourceNotFoundException;

namespace Function.Services;

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
            throw new ResourceNotFoundException("We could not find an account matching the email.");
        }
        catch (NotAuthorizedException)
        {
            throw new AuthenticationException("Incorrect email or password. Try again or create an account.");
        }
        catch (TooManyRequestsException)
        {
            throw new AuthenticationException("Your account is temporarily locked due to multiple failed login attempts. Please try again later.");
        }
    }

    public async Task<string> SignUp(CreateUserCognitoRequest user, string password, Roles role = Roles.Customer)
    {
        var signUpRequest = new SignUpRequest
        {
            ClientId = _clientId,
            Username = user.Email,
            Password = password,
            UserAttributes =
            [
                new AttributeType { Name = "given_name", Value = user.FirstName },
                new AttributeType { Name = "family_name", Value = user.LastName },
                new AttributeType { Name = "email", Value = user.Email }
            ]
        };
        var signUpResponse = await _cognitoClient.SignUpAsync(signUpRequest);
        var confirmRequest = new AdminConfirmSignUpRequest
        {
            UserPoolId = _userPoolId,
            Username = user.Email 
        };

        await _cognitoClient.AdminConfirmSignUpAsync(confirmRequest);

        var updateAttributesRequest = new AdminUpdateUserAttributesRequest
        {
            UserPoolId = _userPoolId,
            Username = user.Email ,
            UserAttributes = [new AttributeType { Name = "custom:role", Value = role.ToString() }]
        };
        
        await _cognitoClient.AdminUpdateUserAttributesAsync(updateAttributesRequest);
        return signUpResponse.UserSub;
    }

    public async Task SignOut(string refreshToken)
    {
        var revokeRequest = new RevokeTokenRequest
        {
            Token = refreshToken,
            ClientId = _clientId
        };

        await _cognitoClient.RevokeTokenAsync(revokeRequest);
    }

    public async Task CheckEmailUniqueness(string email)
    {
        var listUsersRequest = new ListUsersRequest
        {
            UserPoolId = _userPoolId,
            Filter = $"email = \"{email}\""
        };

        var response = await _cognitoClient.ListUsersAsync(listUsersRequest);

        if (response.Users.Count != 0)
        {
            throw new ResourceAlreadyExistsException($"User with email {email} already exists");
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
            throw new UnauthorizedException("Invalid refresh token.", ex);
        }
    }
}