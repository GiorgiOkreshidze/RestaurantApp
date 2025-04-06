using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;

namespace Function.Repository.Interfaces;

public interface ILocationRepository
{
    Task<List<Location>> GetListOfLocationsAsync();
    Task<List<LocationOptions>> GetLocationDropdownOptionsAsync();
    Task<Location> GetLocationByIdAsync(string locationId);
}