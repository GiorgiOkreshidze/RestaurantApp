using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Models.Reservations
{
    public class ReservationsQueryParameters
    {
        public string Date { get; set; } = string.Empty;
        public string TimeFrom { get; set; } = string.Empty;
        public string TableNumber { get; set; } = string.Empty;
    }
}
