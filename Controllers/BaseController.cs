using Google.Apis.Auth.OAuth2;
using JurMaps.Model.DTO;
using JurMaps.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public abstract class BaseController<T> : Controller where T : class
{
    protected readonly IUserService? _userService;
    protected readonly IMapService? _mapService;
    protected readonly ILogger<T>? _logger;
    private readonly IHttpClientFactory? _httpClientFactory;

    protected BaseController(
        IUserService? userService = null,
        ILogger<T>? logger = null,
        IMapService? mapService = null,
        IHttpClientFactory? httpClientFactory = null)
    {
        _userService = userService;
        _logger = logger;
        _mapService = mapService;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Perspective API - Analizuje tekst pod względem toksyczności
    /// </summary>
    /// <param name="text">Tekst, do przeanalizowania</param>
    /// <returns></returns>
    public async Task<IActionResult> AnalyzeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return BadRequest("Text cannot be empty.");
        }

        var apiKey = "AIzaSyANNrfkv56iGfDw3imGSIcgtPD1-iAcyH0";
        var url = $"https://commentanalyzer.googleapis.com/v1alpha1/comments:analyze?key={apiKey}";

        var requestBody = new
        {
            comment = new { text = text },
            requestedAttributes = new { TOXICITY = new { } }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync(url, content);

        var responseString = await response.Content.ReadAsStringAsync();

        var jsonResponse = JsonSerializer.Deserialize<object>(responseString);
        return Ok(jsonResponse);

    }




    /// <summary>
    /// Pobiera serwerowy token Firebase do autoryzacji weryfikacji App Check.
    /// </summary>
    private async Task<string> GetServerTokenAsync()
    {
        var credential = await GoogleCredential.GetApplicationDefaultAsync();

        // Dodaj wymagane scope do poświadczeń OAuth
        credential = credential.CreateScoped(new[]
        {
        "https://www.googleapis.com/auth/cloud-platform",
        "https://www.googleapis.com/auth/firebase"
    });

        var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        return token.Trim();
    }


    /// <summary>
    /// Weryfikuje token Firebase App Check.
    /// </summary>
    protected async Task<IActionResult> VerifyAppCheckTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger?.LogWarning("Brak tokena App Check.");
            return BadRequest("Brak tokena App Check.");
        }

        try
        {
            string serverToken = await GetServerTokenAsync();
            _logger?.LogInformation($"Server token obtained: {serverToken.Substring(0, 10)}...");

            string projectNumber = "661441202308";
            string appId = "1:661441202308:web:b26d1309367d3083839ad1"; // Make sure this is the full app ID

            // Try the correct endpoint
            string url = $"https://firebaseappcheck.googleapis.com/v1beta/projects/{projectNumber}/apps/{appId}:exchangeAppCheckToken";
            _logger?.LogInformation($"Making request to: {url}");

            using var httpClient = _httpClientFactory?.CreateClient() ?? new HttpClient();
            var requestBody = new
            {
                app_check_token = token
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            _logger?.LogInformation($"Request body: {jsonContent}");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serverToken);

            // Log all request headers for debugging
            foreach (var header in request.Headers)
            {
                _logger?.LogInformation($"Request header: {header.Key}: {string.Join(", ", header.Value)}");
            }

            var response = await httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger?.LogInformation($"Response status: {response.StatusCode}");
            _logger?.LogInformation($"Response body: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Nieudana weryfikacja App Check. Status: {response.StatusCode}, Body: {responseBody}");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Błąd weryfikacji tokena App Check.");
            return BadRequest($"Błąd weryfikacji: {ex.Message}");
        }
    }

}
