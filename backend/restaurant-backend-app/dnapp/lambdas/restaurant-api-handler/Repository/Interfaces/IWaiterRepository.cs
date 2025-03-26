using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models.User;

namespace Function.Repository.Interfaces;

public interface IWaiterRepository
{
    Task<List<User>> GetWaitersByLocationAsync(string locationId);
}