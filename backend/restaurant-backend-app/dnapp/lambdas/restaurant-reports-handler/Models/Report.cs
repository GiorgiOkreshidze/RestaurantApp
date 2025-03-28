using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Function.Models
{
    public class Report
    {
        public required string Id { get; set; }
        public required string Location { get; set; }
        public required string Date { get; set; }
        public required string Waiter { get; set; }
        public required string WaiterEmail { get; set; }
        public required int HoursWorked { get; set; }
    }
}
