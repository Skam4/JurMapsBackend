namespace JurMaps.Model.DTO
{
    /// <summary>
    /// DTO zawierające informacje o miejscu
    /// </summary>
    public class PlaceDto
    {
        public int PlaceId { get; set; }
        public string? PlaceName { get; set; }
        public string? PlaceDescription { get; set; }
        public IFormFile? PlacePhoto1 { get; set; }
        public IFormFile? PlacePhoto2 { get; set; }
        public IFormFile? PlacePhoto3 { get; set; }
        public bool ChangePlacePhoto1 { get; set; } = false;
        public bool ChangePlacePhoto2 { get; set; } = false;
        public bool ChangePlacePhoto3 { get; set; } = false;
        public string? PlaceColor { get; set; }
        public string? PlaceCountry { get; set; }
    }
}
