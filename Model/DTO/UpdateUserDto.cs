namespace JurMaps.Model.DTO
{
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int? UserRoleId { get; set; }
    }
}
