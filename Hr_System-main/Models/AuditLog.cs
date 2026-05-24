using System.ComponentModel.DataAnnotations;

namespace Hr_System.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string TableName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Action { get; set; } = null!;

        [Required]
        public string KeyValues { get; set; } = null!;

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        [Required]
        [MaxLength(100)]
        public string ChangedBy { get; set; } = "System";

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
