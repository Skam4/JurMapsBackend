namespace JurMaps.Services.Interfaces
{
    public interface IFirebaseStorageService
    {
        Task DeleteFileAsync(string fileName);

        /// <summary>
        /// Wgrywa zdjęcia do Firebase Storage
        /// </summary>
        /// <param name="file">nazwa pliku</param>
        /// <returns>Publiczny adres zdjęcia</returns>
        /// <exception cref="ArgumentException"></exception>
        Task<string> UploadFileAsync(IFormFile file);

        /// <summary>
        /// Tworzy link dostępu do zdjęcia z nazwy zdjęcia
        /// </summary>
        /// <param name="fileName">Nazwa zdjęcia</param>
        /// <returns></returns>
        Task<string> GetSignedUrlAsync(string fileName);
    }
}
