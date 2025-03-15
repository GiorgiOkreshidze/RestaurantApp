using SimpleLambdaFunction.Services.Interfaces;
using SimpleLambdaFunction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Actions;
using System.Security.Authentication;
using System.Text.Json;
using Function.Models;

namespace Function.Actions
{
    class GetProfileAction
    {
        private readonly IAuthenticationService _authenticationService;

        public GetProfileAction()
        {
            _authenticationService = new AuthenticationService();
        }

        public async Task<APIGatewayProxyResponse> GetProfile(APIGatewayProxyRequest request)
        {
            try
            {
                if (!request.Headers.TryGetValue("X-Access-Token", out var accessTokenHeader) ||
            string.IsNullOrEmpty(accessTokenHeader))
                {
                    return ActionUtils.FormatResponse(401, new { message = "Unauthorized: No access token found." });
                }

                var accessToken = accessTokenHeader.Trim();

                // Fetch user details from Cognito
                var userDetails = await _authenticationService.GetUserDetailsAsync(accessToken);

                // Extract required attributes
                var firstName = userDetails.GetValueOrDefault("given_name");
                var lastName = userDetails.GetValueOrDefault("family_name");
                var email = userDetails.GetValueOrDefault("email");
                var role = userDetails.GetValueOrDefault("custom:role");

                var profileInfo = new ProfileInfo
                {
                    FirstName = firstName!,
                    LastName = lastName!,
                    Email = email!,
                    Role = role!,
                    ImageUrl = "Here should be s3 img url"
                };

                // Return profile data
                return ActionUtils.FormatResponse(200, profileInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching profile: {ex}");
                return ActionUtils.FormatResponse(500, new { message = "An error occurred while fetching the profile." });
            }
        }
    }

}
