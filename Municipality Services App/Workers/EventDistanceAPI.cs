using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Municipality_Services_App.Workers
{
    public class EventDistanceAPI
    {
        public async Task<string> GetDistanceAsync(string origin, string destination)
        {
            if (string.IsNullOrWhiteSpace(origin))
                return "N/A";
            if (string.IsNullOrWhiteSpace(destination))
                return "Unknown";

            const string API_KEY = "AIzaSyDXaQ6hXxnb6lUwouf1pmfn8xqcUBQnKG4";
            const string BASE_URL = "https://maps.googleapis.com/maps/api/distancematrix/json";

            var queryString = new Dictionary<string, string>
            {
                { "origins", Uri.EscapeDataString(origin) },
                { "destinations", Uri.EscapeDataString(destination) },
                { "key", API_KEY }
            };

            var url = $"{BASE_URL}?{string.Join("&", queryString.Select(kv => $"{kv.Key}={kv.Value}"))}";

            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);  // Set a timeout

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();  // Throws if not successful

                var result = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(result);

                if (json["status"]?.ToString() != "OK")
                {
                    throw new Exception($"Google API returned non-OK status: {json["status"]}");
                }

                var elements = json["rows"]?[0]?["elements"] as JArray;
                if (elements == null || !elements.Any())
                {
                    throw new Exception("No route found between the specified origin and destination.");
                }

                var element = elements[0];
                if (element["status"]?.ToString() != "OK")
                {
                    throw new Exception($"Route calculation failed: {element["status"]}");
                }

                return element["distance"]?["text"]?.ToString()
                    ?? throw new Exception("Distance information not found in the response.");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"HTTP Request Error: {e.Message}");
                // You might want to throw here instead of returning a default value
                return "Unable to calculate distance due to network error.";
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON Parsing Error: {e.Message}");
                return "Unable to parse distance information.";
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected Error: {e.Message}");
                return "Unknown";
            }
        }
    }
}
