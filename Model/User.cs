using Microsoft.AspNetCore.Identity;

namespace JurMaps.Model
{
    public class User
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public required string UserPassword { get; set; }
        public string? UserProfilePicture { get; set; }
        public string? UserCreatedDate { get; set; }

        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiration { get; set; }

        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiration { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime? LastVerificationEmailSent { get; set; }
        public int? UserRoleId { get; set; }
        public Role? UserRole { get; set; }
        public List<Map>? UserMaps { get; set; }
        public List<UserMapLike> UserLikedMaps { get; set; } = new List<UserMapLike>();
    }

}
