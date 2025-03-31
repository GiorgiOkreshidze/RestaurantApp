namespace automation_qa.Framework
{
    public class BaseConfiguration
    {
        public static string ApiBaseUrl { get; set; } = "https://08oera2aoc.execute-api.eu-west-2.amazonaws.com/api";
        public static string UiBaseUrl { get; set; } = "https://staging.d1kj6qehyz33js.amplifyapp.com/";
        public static int DefaultTimeout { get; set; } = 10;
        public static string DefaultBrowser { get; set; } = "chrome";
    }
}
