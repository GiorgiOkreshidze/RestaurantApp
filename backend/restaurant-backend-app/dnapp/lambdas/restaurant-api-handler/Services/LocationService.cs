using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Repository;

namespace Function.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    
    public LocationService()
    {
        _locationRepository = new LocationRepository();    
    }
    
    public async Task<Location> GetLocationByIdAsync(string locationId)
    {
        return await _locationRepository.GetLocationByIdAsync(locationId);
    }
    
    public async Task<List<Location>> GetListOfLocationsAsync()
    {
        return await _locationRepository.GetListOfLocationsAsync();
    }

    public async Task<List<LocationOptions>> GetLocationDropdownOptionsAsync()
    {
        return await _locationRepository.GetLocationDropdownOptionsAsync();
    }
}