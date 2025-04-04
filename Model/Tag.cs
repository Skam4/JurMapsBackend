namespace JurMaps.Model
{
    public class Tag
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public int TagQuantity { get; set; } = 0;
        public List<Map> MapsWithThisTag { get; set; } = new List<Map>();
    }
}
