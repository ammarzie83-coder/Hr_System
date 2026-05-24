using System.ComponentModel.DataAnnotations;

namespace Hr_System.Models
{
    public class UserAccount
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "DevelopmentManager";

        public bool CanViewEmployees { get; set; } = true;
        public bool CanAddEmployees { get; set; }
        public bool CanEditEmployees { get; set; }
        public bool CanDeleteEmployees { get; set; }

        public bool CanAddLeaves { get; set; }
        public bool CanEditLeaves { get; set; }
        public bool CanDeleteLeaves { get; set; }

        public bool CanManageAttachments { get; set; }
        public bool CanViewAuditLogs { get; set; }
        public bool CanEditAuditLogs { get; set; }

        public bool IsActive { get; set; } = true;

        // Security fields for lockout handling
        public int FailedLoginAttempts { get; set; } = 0;

        // When set, the account is locked until this UTC time. We still use IsActive
        // to represent administrative activation state; lockout is controlled via FailedLoginAttempts.
        public DateTime? LockoutEndUtc { get; set; }
    }
}
