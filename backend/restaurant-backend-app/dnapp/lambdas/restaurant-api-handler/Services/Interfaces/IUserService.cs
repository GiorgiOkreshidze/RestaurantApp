using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.User;

namespace Function.Services.Interfaces;

public interface IUserService
{
    Task<User> AddUserAsync(User user);
    Task<User> GetUserByIdAsync(string userId);
    Task<List<User>> GetAllCustomersAsync();
}