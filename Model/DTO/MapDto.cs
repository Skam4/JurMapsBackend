namespace JurMaps.Model.DTO
{
    public class MapDto
    {
        public int MapId { get; set; }
        public string? MapName { get; set; }
        public string? MapDescription { get; set; }
        public bool MapUploaded { get; set; } = false;
        public string? MapPublicationDate { get; set; }
        public int? MapLikes { get; set; }
        public string? MapThumbnail { get; set; }
        public int? MapCreatorId { get; set; }
        public string? MapCreatorName { get; set; }
        public string? MapCreatorPicture { get; set; }
        public int? MapPlacesQuantity { get; set; }
        public List<string>? MapTags { get; set; }
        public List<string>? MapOldTags { get; set; }
        public List<string>? MapCountries { get; set; }
    }
}
