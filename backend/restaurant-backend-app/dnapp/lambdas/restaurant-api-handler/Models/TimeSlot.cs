using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Function.Models
{
    public class TimeSlot
    {
        [JsonPropertyName("start")]
        public required string Start { get; set; }

        [JsonPropertyName("end")]
        public required string End { get; set; }
    }
}
