using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using ApiTests.Pages;
using RestSharp;
using automation_qa.Framework;

namespace ApiTests.Utils
{
    public class TestDataHelper : BasePage
    {
        /// <summary>
        /// Creates a random location and returns its ID
        /// </summary>
        public async Task<string> CreateRandomLocation()
        {
            try
            {
                Console.WriteLine("Creating a random test location...");

                var random = new Random();
                var locationData = new
                {
                    id = Guid.NewGuid().ToString(),
                    address = $"{random.Next(100, 999)} Main St, Test City",
                    description = "Test location created for automated testing",
                    totalCapacity = (random.Next(100, 500)).ToString(),
                    averageOccupancy = (random.Next(50, 300)).ToString(),
                    imageUrl = "https://example.com/test-image.jpg",
                    rating = (3.0 + random.NextDouble() * 2.0).ToString("0.0")
                };

                var request = CreatePostRequest("/locations");
                request.AddJsonBody(locationData);
                var response = await ExecutePostRequestAsync(request);
                Console.WriteLine($"Create location response status: {response.StatusCode}");

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    Console.WriteLine($"Successfully created test location with ID: {locationData.id}");
                    return locationData.id;
                }
                else
                {
                    Console.WriteLine($"Failed to create test location. Status: {response.StatusCode}");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating random location: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Fills the database with test locations if no data is present
        /// </summary>
        public async Task<bool> SeedLocationsData()
        {
            try
            {
                Console.WriteLine("Starting data seeding process...");
                Console.WriteLine("Checking for existing locations...");
                var (getStatus, existingLocations) = await GetExistingLocations();
                Console.WriteLine($"Get locations status: {getStatus}");

                if (getStatus == HttpStatusCode.OK && existingLocations?.Count > 0)
                {
                    Console.WriteLine($"Database already contains {existingLocations.Count} locations. Skipping seeding.");
                    return true;
                }
                else if (getStatus == 0)
                {
                    Console.WriteLine("ERROR: Could not connect to the server. Please make sure the API server is running.");
                    Console.WriteLine($"Base URL: {BaseConfiguration.ApiBaseUrl}");

                    var locationsPage = new Locations();
                    var (testStatus, _) = await locationsPage.GetLocations();
                    Console.WriteLine($"Test connection status from Locations class: {testStatus}");

                    return false;
                }

                Console.WriteLine("No locations found in database. Generating test data...");

                var locationsToAdd = GenerateTestLocations();
                Console.WriteLine($"Generated {locationsToAdd.Count} test locations.");

                bool allSucceeded = true;

                for (int i = 0; i < locationsToAdd.Count; i++)
                {
                    var location = locationsToAdd[i];
                    Console.WriteLine($"Adding location {i + 1}/{locationsToAdd.Count}");

                    var (status, response) = await AddLocation(location);
                    Console.WriteLine($"Add location status: {status}, Response: {response?.ToString() ?? "null"}");

                    if (status != HttpStatusCode.Created && status != HttpStatusCode.OK)
                    {
                        Console.WriteLine($"Failed to add test location. Status: {status}");
                        allSucceeded = false;

                        if (status == 0)
                        {
                            Console.WriteLine("ERROR: Could not connect to the server while adding location.");
                            return false;
                        }
                    }
                }

                Console.WriteLine("Verifying added locations...");
                var (verifyStatus, addedLocations) = await GetExistingLocations();
                Console.WriteLine($"Verification status: {verifyStatus}, Found locations: {addedLocations?.Count ?? 0}");

                if (verifyStatus != HttpStatusCode.OK || addedLocations?.Count < locationsToAdd.Count)
                {
                    Console.WriteLine($"Failed to verify that all locations were added. Expected: {locationsToAdd.Count}, Actual: {addedLocations?.Count ?? 0}");
                    allSucceeded = false;
                }
                else
                {
                    Console.WriteLine($"Successfully added {addedLocations.Count} locations to the database.");
                }

                return allSucceeded;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves existing locations from the database
        /// </summary>
        private async Task<(HttpStatusCode StatusCode, JArray? Locations)> GetExistingLocations()
        {
            try
            {
                var request = CreateGetRequest("/locations");
                Console.WriteLine("Executing GET request to /locations...");

                var response = await ExecuteGetRequestAsync(request);
                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response content: {response.Content}");
                Console.WriteLine($"Error message: {response.ErrorMessage}");

                JArray? locations = null;

                if (!string.IsNullOrEmpty(response.Content))
                {
                    try
                    {
                        locations = JArray.Parse(response.Content);
                        Console.WriteLine($"Successfully parsed {locations.Count} locations from response.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing locations response: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Response content is empty.");
                }

                return (response.StatusCode, locations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetExistingLocations: {ex.Message}");
                return (0, null);
            }
        }

        /// <summary>
        /// Adds a new location to the database
        /// </summary>
        private async Task<(HttpStatusCode StatusCode, JObject? Response)> AddLocation(object locationData)
        {
            try
            {
                var request = CreatePostRequest("/locations");

                request.AddJsonBody(locationData);
                Console.WriteLine($"Executing POST request to /locations");

                var response = await ExecutePostRequestAsync(request);
                Console.WriteLine($"Add location response status: {response.StatusCode}");
                Console.WriteLine($"Add location response content: {response.Content}");
                Console.WriteLine($"Add location error message: {response.ErrorMessage}");

                JObject? responseData = null;

                if (!string.IsNullOrEmpty(response.Content))
                {
                    try
                    {
                        responseData = JObject.Parse(response.Content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing add location response: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Add location response content is empty.");
                }

                return (response.StatusCode, responseData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddLocation: {ex.Message}");
                return (0, null);
            }
        }

        /// <summary>
        /// Generates test data for locations
        /// </summary>
        private List<object> GenerateTestLocations()
        {
            return new List<object>
            {
                new
                {
                    id = Guid.NewGuid().ToString(),
                    address = "123 Main St, New York, NY",
                    description = "Nice cozy restaurant in the heart of the city",
                    totalCapacity = "200",
                    averageOccupancy = "150",
                    imageUrl = "https://example.com/images/location1.jpg",
                    rating = "4.5"
                },
                new
                {
                    id = Guid.NewGuid().ToString(),
                    address = "456 Park Ave, Boston, MA",
                    description = "Elegant venue with a view",
                    totalCapacity = "150",
                    averageOccupancy = "100",
                    imageUrl = "https://example.com/images/location2.jpg",
                    rating = "4.8"
                },
                new
                {
                    id = Guid.NewGuid().ToString(),
                    address = "789 Ocean Dr, Miami, FL",
                    description = "Beachfront restaurant with amazing seafood",
                    totalCapacity = "180",
                    averageOccupancy = "120",
                    imageUrl = "https://example.com/images/location3.jpg",
                    rating = "4.2"
                }
            };
        }
    }
}
