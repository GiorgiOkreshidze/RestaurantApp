using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Models
{
    public class SummaryEntry
    {
        public string Location { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string WaiterName { get; set; }
        public string WaiterEmail { get; set; }
        public double CurrentHours { get; set; }
        public double PreviousHours { get; set; }
        public double DeltaHours { get; set; }
    }
}
