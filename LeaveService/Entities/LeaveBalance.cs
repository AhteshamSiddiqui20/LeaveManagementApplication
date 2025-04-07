using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveService.Entities
{
    public class LeaveBalance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int TotalLeaves { get; set; } 

        [Required]
        public int UsedLeaves { get; set; }

        public int RemainingLeaves => TotalLeaves - UsedLeaves;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
