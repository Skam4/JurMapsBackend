namespace JurMaps.Model.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public int UserRoleId { get; set; }
    }
}
