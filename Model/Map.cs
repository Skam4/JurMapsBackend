using System.ComponentModel.DataAnnotations;

namespace JurMaps.Model
{
    public class Map
    {
        [Key]
        public int MapId { get; set; }
        public string? MapName { get; set; }
        public string? MapDescription { get; set; }
        public bool MapUploaded { get; set; } = false;
        public string? MapPublicationDate { get; set; }
        public string? MapCreationDate { get; set; }
        public int? MapLikes { get; set; } = 0;
        public int? MapPlacesQuantity { get; set; } = 0;
        public string? MapThumbnail { get; set; }
        public int MapCreatorId { get; set; }
        public User? MapCreator { get; set; }
        public List<Tag> MapTags { get; set; } = new List<Tag>();
        public List<Place> MapPlaces { get; set; } = new List<Place>();
        public List<MapCountry> MapCountries { get; set; } = new List<MapCountry>();

        public List<UserMapLike> UserLikes { get; set; } = new List<UserMapLike>();
    }
}
