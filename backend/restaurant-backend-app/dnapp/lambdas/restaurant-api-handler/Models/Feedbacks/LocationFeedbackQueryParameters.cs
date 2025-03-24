using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Models
{
    public class LocationFeedbackQueryParameters
    {
        public required string LocationId { get; set; }
        public LocationFeedbackType? Type { get; set; }
        public required string SortProperty { get; set; }
        public required string SortDirection { get; set; }
        public int PageSize { get; set; }
        public string? NextPageToken { get; set; }
    }
}
