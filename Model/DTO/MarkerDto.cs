namespace JurMaps.Model.DTO
{
    public class MarkerDto
    {
        public int? MarkerId { get; set; }
        public PositionDto Position { get; set; } = new PositionDto();
        public string? MarkerName { get; set; }
        public string? MarkerDescription { get; set; }
        public string? PlacePhoto1 { get; set; }
        public string? PlacePhoto2 { get; set; }
        public string? PlacePhoto3 { get; set; }
        public string? PlaceColor { get; set; }
        public string? PlaceCountry { get; set; }
    }
}
