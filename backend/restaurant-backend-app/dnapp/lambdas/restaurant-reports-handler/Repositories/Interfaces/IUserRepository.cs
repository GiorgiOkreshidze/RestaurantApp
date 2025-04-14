using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<string> GetUserFullName(string email);

        public Task<string> GetUserEmail(string id);
    }
}
