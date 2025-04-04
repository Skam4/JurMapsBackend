namespace JurMaps.Model.DTO
{
    /// <summary>
    /// Używane w ConfirmResetPassword, do resetowania hasła
    /// </summary>
    public class ConfirmResetPasswordRequest
    {
        /// <summary>
        /// Token wysyłany na maila generowany do weryfikacji
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Nowe hasło wpisane przez użytkownika
        /// </summary>
        public string? NewPassword { get; set; }
    }
}
