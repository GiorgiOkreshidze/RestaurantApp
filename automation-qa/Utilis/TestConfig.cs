using System.IO;
using Newtonsoft.Json;

namespace ApiTests.Utilities
{
    public class TestConfig
    {
        public string TestUserEmail { get; set; }
        public string TestUserPassword { get; set; }
        public string AdminUserEmail { get; set; }
        public string AdminUserPassword { get; set; }
        public string ValidLocationId { get; set; }
        public string ValidReservationId { get; set; }
        public string WaiterEmail { get; set; }
        public string WaiterPassword { get; set; }

        private static TestConfig _instance;
        public static TestConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadConfig();
                }
                return _instance;
            }
        }

        private static TestConfig LoadConfig()
        {
            string configFile = "testConfig.json";

            if (!File.Exists(configFile))
            {
                configFile = Path.Combine(Directory.GetCurrentDirectory(), "testConfig.json");
            }

            if (File.Exists(configFile))
            {
                string json = File.ReadAllText(configFile);
                return JsonConvert.DeserializeObject<TestConfig>(json);
            }

            return new TestConfig
            {
                TestUserEmail = "test@example.com",
                TestUserPassword = "StrongP@ss123!",
                AdminUserEmail = "admin@example.com",
                AdminUserPassword = "AdminP@ss123!",
                ValidLocationId = "8c4fc44e-c1a5-42eb-9912-55aeb5111a99",
                ValidReservationId = "43a479ef-4aa0-4472-b72e-7833f90a591",
                WaiterEmail = "waiter@restaurant.com",
                WaiterPassword = "WaiterP@ss123!"
            };
        }
    }
}