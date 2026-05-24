using Hr_System.Data;
using Hr_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Hr_System.Pages.Account
{
    [Authorize(Roles = "Informatics")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string DisplayName { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public bool IsInformatics { get; set; }
            public bool IsActive { get; set; }
            public bool CanViewEmployees { get; set; }
            public bool CanAddEmployees { get; set; }
            public bool CanEditEmployees { get; set; }
            public bool CanDeleteEmployees { get; set; }
            public bool CanAddLeaves { get; set; }
            public bool CanEditLeaves { get; set; }
            public bool CanDeleteLeaves { get; set; }
            public bool CanManageAttachments { get; set; }
            public bool CanViewAuditLogs { get; set; }
            public bool CanEditAuditLogs { get; set; }
            [DataType(DataType.Password)]
            [MinLength(8, ErrorMessage = "يجب أن تكون كلمة المرور الجديدة 8 أحرف على الأقل.")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
            public string? ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _db.UserAccounts.FindAsync(Id);
            if (user == null)
            {
                return NotFound();
            }

            Username = user.Username;
            Input.DisplayName = user.DisplayName;
            Input.Role = user.Role;
            Input.IsInformatics = string.Equals(user.Role, "Informatics", StringComparison.OrdinalIgnoreCase);
            Input.IsActive = user.IsActive;
            Input.CanViewEmployees = user.CanViewEmployees;
            Input.CanAddEmployees = user.CanAddEmployees;
            Input.CanEditEmployees = user.CanEditEmployees;
            Input.CanDeleteEmployees = user.CanDeleteEmployees;
            Input.CanAddLeaves = user.CanAddLeaves;
            Input.CanEditLeaves = user.CanEditLeaves;
            Input.CanDeleteLeaves = user.CanDeleteLeaves;
            Input.CanManageAttachments = user.CanManageAttachments;
            Input.CanViewAuditLogs = user.CanViewAuditLogs;
            Input.CanEditAuditLogs = user.CanEditAuditLogs;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _db.UserAccounts.FindAsync(Id);
            if (user == null)
            {
                return NotFound();
            }

            user.DisplayName = Input.DisplayName;
            if (Input.IsInformatics || string.Equals(Input.Role, "Informatics", StringComparison.OrdinalIgnoreCase))
            {
                user.Role = "Informatics";
                user.CanViewEmployees = true;
                user.CanAddEmployees = true;
                user.CanEditEmployees = true;
                user.CanDeleteEmployees = true;
                user.CanAddLeaves = true;
                user.CanEditLeaves = true;
                user.CanDeleteLeaves = true;
                user.CanManageAttachments = true;
                user.CanViewAuditLogs = true;
                user.CanEditAuditLogs = true;
            }
            else
            {
                user.Role = Input.Role;
                user.CanViewEmployees = Input.CanViewEmployees;
                user.CanAddEmployees = Input.CanAddEmployees;
                user.CanEditEmployees = Input.CanEditEmployees;
                user.CanDeleteEmployees = Input.CanDeleteEmployees;
                user.CanAddLeaves = Input.CanAddLeaves;
                user.CanEditLeaves = Input.CanEditLeaves;
                user.CanDeleteLeaves = Input.CanDeleteLeaves;
                user.CanManageAttachments = Input.CanManageAttachments;
                user.CanViewAuditLogs = Input.CanViewAuditLogs;
                user.CanEditAuditLogs = Input.CanEditAuditLogs;
            }
            user.IsActive = Input.IsActive;
            // If a new password is provided, validate strength and update it.
            if (!string.IsNullOrWhiteSpace(Input.NewPassword))
            {
                if (Input.NewPassword.Length < 8 || !Input.NewPassword.Any(char.IsDigit) || !Input.NewPassword.Any(char.IsLetter))
                {
                    ModelState.AddModelError(string.Empty, "كلمة المرور غير كافية: يجب أن تكون 8 أحرف على الأقل وتحتوي على حروف وأرقام.");
                    return Page();
                }

                user.PasswordHash = PasswordHelper.HashPassword(Input.NewPassword);
            }
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"تم تحديث حساب {user.Username} بنجاح.";
            return RedirectToPage("Manage");
        }
    }
}
