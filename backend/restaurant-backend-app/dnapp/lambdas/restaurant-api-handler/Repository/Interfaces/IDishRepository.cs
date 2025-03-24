using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;

namespace Function.Repository.Interfaces;

public interface IDishRepository
{
    Task<List<Dish>> GetListOfPopularDishesAsync();
    Task<List<Dish>> GetListOfSpecialityDishesAsync(string locationId);
}