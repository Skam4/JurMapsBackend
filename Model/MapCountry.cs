namespace JurMaps.Model
{
    public class MapCountry
    {
        public int MapId { get; set; }
        public int CountryId { get; set; }
        public Map Map { get; set; } = null!;
        public Country Country { get; set; } = null!;

        public int ConnectionCount { get; set; }
    }
}
