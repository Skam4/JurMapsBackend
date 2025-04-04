namespace JurMaps.Model.DTO
{
    /// <summary>
    /// DTO dla zmiany hasła użytkownika
    /// </summary>
    public class ChangeUserPasswordDto
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
