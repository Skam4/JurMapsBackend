namespace JurMaps.Model.DTO
{
    public class RectangleDto
    {
        public int? RectangleId { get; set; }
        public List<PositionDto> Bounds { get; set; }
        public string? RectangleName { get; set; }
        public string? RectangleDescription { get; set; }
        public string? PlacePhoto1 { get; set; }
        public string? PlacePhoto2 { get; set; }
        public string? PlacePhoto3 { get; set; }
    }
}
