namespace automation_qa.Framework
{
    public class BaseConfiguration
    {
        public static string ApiBaseUrl { get; set; } = "https://xzi78ndtre.execute-api.eu-west-2.amazonaws.com/api";
        public static string UiBaseUrl { get; set; } = "http://localhost:5174/";
        public static int DefaultTimeout { get; set; } = 10;
        public static string DefaultBrowser { get; set; } = "chrome";
    }
}
