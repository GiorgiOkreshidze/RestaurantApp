using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;

namespace Function.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    
    public TableService()
    {
        _tableRepository = new TableRepository();    
    }
    
    public async Task<List<RestaurantTable>> GetTablesForLocationAsync(string locationId, int guests)
    {
        return await _tableRepository.GetTablesForLocationAsync(locationId, guests);
    }
}