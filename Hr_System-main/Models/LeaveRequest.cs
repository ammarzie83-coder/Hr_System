using System.ComponentModel.DataAnnotations;

namespace Hr_System.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public virtual Employee? Employee { get; set; }

        [Required]
        [MaxLength(80)]
        public string LeaveType { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [MaxLength(300)]
        public string Reason { get; set; } = null!;

        [Required]
        [MaxLength(40)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
