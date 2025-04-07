using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService.Entities
{
    public class AhteshamSohaibusers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual AhteshamSohaibUserRoles AhteshamSohaibUserRoles { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
