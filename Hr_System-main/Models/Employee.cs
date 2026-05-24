using System.ComponentModel.DataAnnotations;

namespace Hr_System.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string NationalId { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string JobTitle { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Mobile { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(120)]
        public string Email { get; set; } = null!;

        [Required]
        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        public byte[]? PhotoData { get; set; }

        [MaxLength(100)]
        public string? PhotoContentType { get; set; }

        public virtual ICollection<EmployeeAttachment> Attachments { get; set; } = new List<EmployeeAttachment>();

        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
