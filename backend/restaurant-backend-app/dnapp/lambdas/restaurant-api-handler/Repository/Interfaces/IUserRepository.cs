using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.User;

namespace Function.Repository.Interfaces;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
    Task<User> GetUserByIdAsync(string userId);
    Task<List<User>> GetAllUsersAsync();
}