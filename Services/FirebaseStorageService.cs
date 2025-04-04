using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using System;
using System.IO;
using System.Threading.Tasks;
using JurMaps.Services.Interfaces;

public class FirebaseStorageService : IFirebaseStorageService
{
    private readonly StorageClient _storageClient;
    private const string BucketName = "jurmaps.appspot.com";
    string credentialsPath;

    public FirebaseStorageService()
    {
        credentialsPath = "jurmaps-f9f3a5c73a1b.json";

        // Upewnij się, że plik istnieje przed próbą załadowania
        if (!File.Exists(credentialsPath))
        {
            throw new FileNotFoundException($"Nie znaleziono pliku z kluczami: {credentialsPath}");
        }

        // Ustawienie zmiennej środowiskowej (opcjonalnie)
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.GetFullPath(credentialsPath));

        var credential = GoogleCredential.FromFile(credentialsPath);
        _storageClient = StorageClient.Create(credential);
    }

    /// <summary>
    /// Usuwa plik z firebase Storage po nazwie
    /// </summary>
    /// <param name="fileUrl">nazwa zdjęcia</param>
    /// <returns></returns>
    public async Task DeleteFileAsync(string fileName)
    {
        try
        {
            await _storageClient.DeleteObjectAsync(BucketName, $"images/{fileName}");
            Console.WriteLine($"✅ Plik {fileName} został usunięty.");
        }
        catch (Google.GoogleApiException e)
        {
            if (e.Error.Code == 404)
            {
                Console.WriteLine($"⚠️ Plik {fileName} nie istnieje w Firebase Storage.");
            }
            else
            {
                Console.WriteLine($"❌ Błąd usuwania pliku: {e.Error.Message}");
                throw;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Nieoczekiwany błąd: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Wgrywa zdjęcia do Firebase Storage
    /// </summary>
    /// <param name="file">nazwa pliku</param>
    /// <returns>Publiczny adres zdjęcia</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Plik jest pusty lub nie został przesłany.");
        }

        var random = new Random().Next(1, 999);

        var uniqueFileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{random}";

        using (var stream = file.OpenReadStream())
        {
            var storageObject = await _storageClient.UploadObjectAsync(
                BucketName,
                $"images/{uniqueFileName}",
                file.ContentType,
                stream);

            Console.WriteLine($"✅ Plik {uniqueFileName} został przesłany do Firebase Storage.");
        }

        return uniqueFileName;
    }

    /// <summary>
    /// Tworzy link dostępu do zdjęcia z nazwy zdjęcia
    /// </summary>
    /// <param name="fileName">Nazwa zdjęcia</param>
    /// <returns></returns>
    public async Task<string> GetSignedUrlAsync(string fileName)
    {
        var urlSigner = UrlSigner.FromServiceAccountPath(credentialsPath);
        string signedUrl = await urlSigner.SignAsync(
            bucket: BucketName,
            objectName: $"images/{fileName}",
            duration: TimeSpan.FromHours(1)
        );

        return signedUrl;
    }


    public static string ExtractFileNameFromUrl(string url)
    {
        var decodedUrl = System.Net.WebUtility.UrlDecode(url);
        var uri = new Uri(decodedUrl);
        var segments = uri.AbsolutePath.Split('/');
        var fileName = segments[^1];

        if (fileName.Contains("?"))
        {
            fileName = fileName.Split('?')[0];
        }

        return fileName;
    }
}
