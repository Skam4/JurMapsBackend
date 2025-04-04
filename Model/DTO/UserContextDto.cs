namespace JurMaps.Model.DTO
{
    public class UserContextDto
    {
        public bool isLogged {  get; set; } = false;
        public bool isAdmin { get; set; } = false;
        public string? profilePicture {  get; set; }
        public int userId { get; set; }
    }
}
