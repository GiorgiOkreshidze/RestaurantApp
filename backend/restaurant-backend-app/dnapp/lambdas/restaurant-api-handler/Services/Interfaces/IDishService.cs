using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Models.Dishes;
using Function.Models.Responses;

namespace Function.Services.Interfaces;

public interface IDishService
{
    Task<DishResponseDto> GetDishByIdAsync(string dishId);
    Task<List<Dish>> GetListOfPopularDishesAsync();
    Task<List<Dish>> GetListOfSpecialityDishesAsync(string locationId);
    Task<IEnumerable<DishResponseDto>> GetAllDishAsync();
}