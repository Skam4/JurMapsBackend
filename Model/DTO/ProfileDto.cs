namespace JurMaps.Model.DTO {

    /// <summary>
    /// DTO dla danych profilu użytkownika
    /// </summary>
    public class ProfileDto
    {
        public string UserName { get; set; }
        public string? UserProfilePicture { get; set; }
        public string? UserCreatedDate { get; set; }
        public List<MapDto>? UserPublicatedMaps { get; set; }
        public List<MapDto>? UserLikedMaps { get; set; }

    }
}
