using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;

namespace Function.Services.Interfaces;

public interface ITableService
{
    Task<List<RestaurantTable>> GetTablesForLocationAsync(string locationId, int guests);
}