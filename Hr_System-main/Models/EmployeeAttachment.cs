using System.ComponentModel.DataAnnotations;

namespace Hr_System.Models
{
    public class EmployeeAttachment
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public virtual Employee? Employee { get; set; }

        [Required]
        [MaxLength(200)]
        public string FileName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = null!;

        [Required]
        public byte[] Data { get; set; } = null!;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
