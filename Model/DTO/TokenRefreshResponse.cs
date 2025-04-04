namespace JurMaps.Model.DTO
{
    /// <summary>
    /// Wykorzystywany w RefreshTokenAsync, podczas odświeżania tokenu
    /// </summary>
    public class TokenRefreshResponse
    {
        /// <summary>
        /// Token JWT użytkownika
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// ID użytkownika
        /// </summary>
        public string UserId { get; set; }
    }
}
