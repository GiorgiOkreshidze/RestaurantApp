﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.Requests;
using Function.Models.Responses;
using Function.Models.User;

namespace Function.Services.Interfaces;

public interface IAuthenticationService
{
    Task<AuthResult> SignIn(string email, string password);

    Task<string> SignUp(CreateUserCognitoRequest user, string password, Roles role = Roles.Customer);

    Task SignOut(string accessToken);

    Task CheckEmailUniqueness(string email);

    Task<AuthResult> RefreshToken(string refreshToken);
}