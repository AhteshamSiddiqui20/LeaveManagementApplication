using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LeaveService.Model
{
    public class LeaveDTO
    {
        public int EmployeeId { get; set; }

        [Required]
        public int TotalLeaves { get; set; }

        [Required]
        public int UsedLeaves { get; set; }

        
    }
}
