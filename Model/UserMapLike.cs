namespace JurMaps.Model
{
    public class UserMapLike
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int MapId { get; set; }
        public Map Map { get; set; }
    }

}
