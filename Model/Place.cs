using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace JurMaps.Model
{
    public class Place
    {
        [Key]
        public int PlaceId { get; set; }
        public string? PlaceName { get; set; }
        public string? PlaceType { get; set; }
        public Point? PlacePosition { get; set; }
        //public double? PlaceRadius { get; set; }
        //public Polygon? Bounds { get; set; }

        public string? PlacePhoto1 { get; set; }
        public string? PlacePhoto2 { get; set; }
        public string? PlacePhoto3 { get; set; }
        public string? PlaceColor {  get; set; }
        public string? PlaceCountry { get; set; }
        public int MapId { get; set; }
        public Map PlaceMap { get; set; }
        public string? PlaceDescription { get; set; }
    }
}
