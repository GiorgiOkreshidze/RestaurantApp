using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Models
{
    public static class LocationFeedbackTypeExtensions
    {
        public static string ToDynamoDBType(this LocationFeedbackType feedbackType)
        {
            return feedbackType switch
            {
                LocationFeedbackType.ServiceQuality => "SERVICE_QUALITY",
                LocationFeedbackType.CuisineExperience => "CUISINE_EXPERIENCE",
                _ => throw new ArgumentException("Unknown feedback type")
            };
        }

        public static LocationFeedbackType ToLocationFeedbackType(this string type)
        {
            return type switch
            {
                "SERVICE_QUALITY" => LocationFeedbackType.ServiceQuality,
                "CUISINE_EXPERIENCE" => LocationFeedbackType.CuisineExperience,
                _ => throw new ArgumentException($"Invalid feedback type: {type}", nameof(type))
            };
        }
    }
}
