namespace JurMaps.Model.DTO
{
    public class CircleDto
    {
        public int? CircleId { get; set; }
        public PositionDto Position { get; set; } = new PositionDto();
        public double Radius { get; set; }
        public string? CircleName { get; set; }
        public string? CircleDescription { get; set; }
        public string? PlacePhoto1 { get; set; }
        public string? PlacePhoto2 { get; set; }
        public string? PlacePhoto3 { get; set; }
    }
}
