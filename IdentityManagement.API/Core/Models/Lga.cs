namespace IdentityManagement.API.Core.Models
{
    public class Lga : BaseEntity
    {
        public string Name { get; set; }
        public int StateId { get; set; }
        public virtual State State { get; set; }
    }
}
