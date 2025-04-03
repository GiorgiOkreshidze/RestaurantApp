using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Function.Models
{
    public class EventWrapper
    {
        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("payload")]
        public JsonElement Payload { get; set; }
    }
}
