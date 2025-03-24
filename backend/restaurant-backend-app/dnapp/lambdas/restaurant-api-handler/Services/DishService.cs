using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;

namespace Function.Services;

public class DishService : IDishService
{
    private readonly IDishRepository _dishRepository;
    
    public DishService()
    {
        _dishRepository = new DishRepository();    
    }

    public async Task<List<Dish>> GetListOfPopularDishesAsync()
    {
        return await _dishRepository.GetListOfPopularDishesAsync();
    }

    public async Task<List<Dish>> GetListOfSpecialityDishesAsync(string locationId)
    {
        return await _dishRepository.GetListOfSpecialityDishesAsync(locationId);
    }
}