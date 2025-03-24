using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;

namespace Function.Services.Interfaces;

public interface ILocationService
{
    Task<List<Location>> GetListOfLocationsAsync();
    Task<List<LocationOptions>> GetLocationDropdownOptionsAsync();
    Task<Location> GetLocationByIdAsync(string locationId);
    Task<LocationInfo> GetLocationDetailsAsync(string locationId);
}