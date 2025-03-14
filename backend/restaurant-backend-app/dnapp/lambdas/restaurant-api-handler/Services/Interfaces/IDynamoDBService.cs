using Function.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Services.Interfaces
{
    public interface IDynamoDBService
    {
        public Task<bool> CheckIfEmailExistsInWaitersTable(string email);

        public Task<List<Location>> GetListOfLocations();
    }
}
