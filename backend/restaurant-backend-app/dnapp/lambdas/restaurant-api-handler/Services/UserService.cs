using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Exceptions;
using Function.Models.Responses;
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

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetUserByEmailAsync(email);
    }

    public async Task<List<User>> GetAllCustomersAsync()
    {
        return await _userRepository.GetAllCustomersAsync();
    }

    public async Task<UserResponse> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user is null)
        {
            throw new ResourceNotFoundException("The requested resource could not be found.");
        }

        var userDto = new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.ToString(),
            ImageUrl = user.ImageUrl
        };

        return userDto;
    }

}