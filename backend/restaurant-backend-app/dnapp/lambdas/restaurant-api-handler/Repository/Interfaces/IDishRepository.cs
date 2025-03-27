using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.Dishes;
using Function.Models.Requests;
using Function.Models.Responses;

namespace Function.Repository.Interfaces;

public interface IDishRepository
{
    Task<List<Dish>> GetListOfPopularDishesAsync();
    Task<List<Dish>> GetListOfSpecialityDishesAsync(string locationId);
    Task<IEnumerable<AllDishResponseDto>> GetAllDishAsync(GetallDishRequest getallDishRequest);
    Task<ExactDishResponseDto> GetDishByIdAsync(string dishId);
}