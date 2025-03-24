using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;

namespace Function.Repository.Interfaces;

public interface ITableRepository
{
    Task<List<RestaurantTable>> GetTablesForLocationAsync(string locationId, int guests);
}