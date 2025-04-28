namespace automation_qa.Framework
{
    public class BaseConfiguration
    {
        public static string ApiBaseUrl { get; set; } = "https://restaurants-run7team2-api-handler-dev.development.krci-dev.cloudmentor.academy/api";
        public static string UiBaseUrl { get; set; } = " http://localhost:5173//";
        public static int DefaultTimeout { get; set; } = 10;
        public static string DefaultBrowser { get; set; } = "chrome";
    }
}
