using System.Net.Http;
using System.Text.Json;

namespace DineConnect.App.Services
{
    // DTOs to match the Google Places APIJSON response
    public class Prediction
    {
        public string description { get; set; }
        public string place_id { get; set; }
    }

    public class AutocompleteResult
    {
        public List<Prediction> predictions { get; set; }
        public string status { get; set; }
    }

    public class GooglePlacesService
    {
        private const string ApiKey = "AIzaSyBwKOrbPdEu5GxuFoj_P4TTUWXUvt4cPAk";
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<IEnumerable<Prediction>> GetAutocompleteSuggestionAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(ApiKey) || ApiKey == "AIzaSyBwKOrbPdEu5GxuFoj_P4TTUWXUvt4cPAk")
            {
                return Enumerable.Empty<Prediction>();
            }

            // This URL searches for establishments (like restaurants) in Syden, Australia by default.
            // You can change locatoin and radius to better suit your needs.

            string requestUrl = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={input}&types=establishment&location=-33.8688,151.2093&radius=50000&key={ApiKey}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<Prediction>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AutocompleteResult>(json);

            return result?.predictions ?? Enumerable.Empty<Prediction>();
        }
    }
}
