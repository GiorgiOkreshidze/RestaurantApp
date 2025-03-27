namespace automation_qa.Framework
{
    public class BaseConfiguration
    {
        public static string ApiBaseUrl { get; set; } = "https://apc79c27sb.execute-api.eu-west-2.amazonaws.com/api";
        public static int DefaultTimeout { get; set; } = 10;
        public static string DefaultBrowser { get; set; } = "chrome";
    }
}
