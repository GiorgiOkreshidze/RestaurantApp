using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Function.Services.Interfaces;

public interface ITableService
{
    Task<List<TableResponse>> GetAvailableTablesAsync(string locationId, string date, string? time, int guests);
}