using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;

namespace Function.Services.Interfaces;

public interface IDishService
{
    Task<List<Dish>> GetListOfPopularDishesAsync();
    Task<List<Dish>> GetListOfSpecialityDishesAsync(string locationId);
}