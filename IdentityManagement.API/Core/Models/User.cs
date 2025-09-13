namespace IdentityManagement.API.Core.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public virtual Lga? Lga { get; set; }
        public int? LgaId { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
