using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService.Model
{
    public class LeaveApplyDTO
    {
        public int EmployeeId { get; set; }

        [Required]
        public int TotalLeaves { get; set; }

        [Required]
        public int UsedLeaves { get; set; }

        
    }
}
