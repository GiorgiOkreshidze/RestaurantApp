using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.User;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;

namespace Function.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService()
    {
        _userRepository = new UserRepository();
    }
    
    public async Task<User> AddUserAsync(User user)
    {
        return await _userRepository.AddUserAsync(user);
    }

    public async Task<User> GetUserByIdAsync(string userId)
    {
        return await _userRepository.GetUserByIdAsync(userId);
    }

    public async Task<List<User>> GetAllCustomersAsync()
    {
        return await _userRepository.GetAllCustomersAsync();
    }
}