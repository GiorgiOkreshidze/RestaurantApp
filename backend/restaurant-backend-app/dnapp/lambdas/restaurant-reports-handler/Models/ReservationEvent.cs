using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Models
{
    public class Event<T>
    {
        public required string EventType { get; set; }
        
        public required T Payload { get; set; }
    }
}
