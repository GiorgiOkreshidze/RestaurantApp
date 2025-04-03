using Amazon.DynamoDBv2.Model;
using Function.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Repositories.Interfaces
{
    public interface IReportRepository
    {
        public Task<List<Dictionary<string, AttributeValue>>> RetrieveReports(DateTime startDate, DateTime endDate); 
    }
}
