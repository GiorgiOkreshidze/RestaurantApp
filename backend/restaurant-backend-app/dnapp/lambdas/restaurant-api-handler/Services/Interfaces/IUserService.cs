using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.Responses;
using Function.Models.User;

namespace Function.Services.Interfaces;

public interface IUserService
{
    Task<User> AddUserAsync(User user);

    Task<User> GetUserByEmailAsync(string email);

    Task<List<User>> GetAllCustomersAsync();

    Task<UserResponse> GetUserByIdAsync(string id);
}