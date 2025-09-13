using System.ComponentModel.DataAnnotations;

namespace IdentityManagement.API.Core.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
