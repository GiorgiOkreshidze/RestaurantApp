using Function.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Repositories.Interfaces
{
    public interface IReportRepository
    {
        public Task<List<Report>> RetrieveReports(DateTime startDate, DateTime endDate); 
    }
}
