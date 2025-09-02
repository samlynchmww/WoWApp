using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WoWApp.Services
{
    public class BlizzardApiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private string? _accessToken;

        private readonly string[] _regions = new[] { "us", "eu", "kr", "tw" };

        public BlizzardApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken))
                return _accessToken;

            var clientId = _config["BlizzardApi:ClientId"];
            var clientSecret = _config["BlizzardApi:ClientSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                throw new InvalidOperationException("Blizzard API ClientId or ClientSecret is missing.");

            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            var authHeader = Convert.ToBase64String(authBytes);

            var tokenUrl = "https://us.battle.net/oauth/token";
            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("grant_type", "client_credentials")
            });

            var response = await _http.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to get access token: {response.StatusCode} - {content}");

            using var doc = JsonDocument.Parse(content);
            _accessToken = doc.RootElement.GetProperty("access_token").GetString();

            return _accessToken!;
        }

        /// <summary>
        /// Attempts to fetch character JSON; returns null if not found, along with region for fallback.
        /// </summary>
        public async Task<(string? Json, string Region)> GetCharacterAsync(string realm, string characterName)
        {
            realm = Slugify(realm);
            characterName = Slugify(characterName);

            var token = await GetAccessTokenAsync();

            foreach (var region in _regions)
            {
                var url = $"https://{region}.api.blizzard.com/profile/wow/character/{realm}/{characterName}?namespace=profile-{region}&locale=en_US&access_token={token}";
                var response = await _http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return (json, region);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    continue;
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error fetching character in region {region}: {response.StatusCode} - {err}");
                }
            }

            return (null, "us"); // fallback region for Armory link
        }

        private string Slugify(string input)
        {
            return input.ToLowerInvariant()
                        .Replace("'", "")
                        .Replace(" ", "-");
        }
    }
}
