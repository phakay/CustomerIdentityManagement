namespace IdentityManagement.API.Core.Models
{
    public class State : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Lga> Lgas { get; set; } = [];
    }
}
