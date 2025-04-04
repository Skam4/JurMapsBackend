using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JurMaps.Shared
{
    public class Helpers
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public Helpers(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration;
        }

        /// <summary>
        /// Analizuje tekst pod kątem obraźliwych treści
        /// </summary>
        /// <param name="text">Tekst, który zostanie objęty wglądem</param>
        /// <returns>Poziom toksyczności tekstu</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<float> AnalyzeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty.");

            var apiKey = _configuration["GoogleApiKey"];
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

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Tutaj można sprawdzić, czy błąd dotyczy detekcji języka
                if (errorContent.Contains("language"))
                {
                    // Próbujemy ponownie z domyślnym językiem, np. "en" / "pl"
                    var requestBody2 = new
                    {
                        comment = new { text = text },
                        languages = new[] { "en" },
                        requestedAttributes = new { TOXICITY = new { } }
                    };

                    json = JsonSerializer.Serialize(requestBody2);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(url, content);
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"API request failed with status code {response.StatusCode}. Details: {await response.Content.ReadAsStringAsync()}");
                }
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseString);
            var toxicityValue = jsonDoc.RootElement
                .GetProperty("attributeScores")
                .GetProperty("TOXICITY")
                .GetProperty("summaryScore")
                .GetProperty("value")
                .GetSingle();

            return toxicityValue;
        }

        /// <summary>
        /// Sprawdza czy zdjęcie nie zawiera wrażliwych treści
        /// </summary>
        /// <param name="imageFile">Plik zdjęcia</param>
        public async Task CheckImageAsync(IFormFile imageFile)
        {
            if (imageFile == null)
            {
                // Obsługa sytuacji, gdy brak przesłanego pliku
                return;
            }

            // Utworzenie poświadczeń z pliku JSON
            var credential = GoogleCredential.FromFile(_configuration["GOOGLE_APPLICATION_CREDENTIALS"]);

            // Utworzenie klienta przy użyciu poświadczeń
            var client = new ImageAnnotatorClientBuilder
            {
                Credential = credential,
            }.Build();

            using (var stream = imageFile.OpenReadStream())
            {
                var image = Image.FromStream(stream);

                // Asynchroniczne wywołanie metody SafeSearch Detection
                var safeSearch = await client.DetectSafeSearchAsync(image);

                if (safeSearch.Adult >= Likelihood.Likely ||
                    safeSearch.Violence >= Likelihood.Likely ||
                    safeSearch.Racy >= Likelihood.Likely)
                {
                    throw new InvalidOperationException("Zdjęcie zawiera potencjalnie nieodpowiednie treści.");
                }
            }
        }

    }
}
