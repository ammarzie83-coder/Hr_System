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
    public class ManageModel : PageModel
    {
        private readonly AppDbContext _db;

        public ManageModel(AppDbContext db)
        {
            _db = db;
        }

        public List<UserAccount> Users { get; set; } = new();

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "الاسم الظاهر مطلوب.")]
            public string DisplayName { get; set; } = string.Empty;

            public string Role { get; set; } = "Viewer";
            public bool IsInformatics { get; set; }
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
        }

        public async Task OnGetAsync()
        {
            Users = await _db.UserAccounts.OrderBy(u => u.Username).ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (string.IsNullOrWhiteSpace(Input.Username) || string.IsNullOrWhiteSpace(Input.Password) || string.IsNullOrWhiteSpace(Input.DisplayName))
            {
                ModelState.AddModelError(string.Empty, "يرجى ملء الحقول المطلوبة.");
                await OnGetAsync();
                return Page();
            }

            // Basic password strength enforcement
            if (Input.Password.Length < 8 || !Input.Password.Any(char.IsDigit) || !Input.Password.Any(char.IsLetter))
            {
                ModelState.AddModelError(string.Empty, "كلمة المرور غير كافية: يجب أن تكون 8 أحرف على الأقل وتحتوي على حروف وأرقام.");
                await OnGetAsync();
                return Page();
            }

            if (await _db.UserAccounts.AnyAsync(u => u.Username == Input.Username))
            {
                ModelState.AddModelError(string.Empty, "اسم المستخدم مستخدم بالفعل.");
                await OnGetAsync();
                return Page();
            }

            var user = new UserAccount
            {
                Username = Input.Username,
                PasswordHash = PasswordHelper.HashPassword(Input.Password),
                DisplayName = Input.DisplayName,
                Role = Input.IsInformatics ? "Informatics" : Input.Role,
                CanViewEmployees = Input.CanViewEmployees,
                CanAddEmployees = Input.CanAddEmployees,
                CanEditEmployees = Input.CanEditEmployees,
                CanDeleteEmployees = Input.CanDeleteEmployees,
                CanAddLeaves = Input.CanAddLeaves,
                CanEditLeaves = Input.CanEditLeaves,
                CanDeleteLeaves = Input.CanDeleteLeaves,
                CanManageAttachments = Input.CanManageAttachments,
                CanViewAuditLogs = Input.CanViewAuditLogs,
                CanEditAuditLogs = Input.CanEditAuditLogs,
                IsActive = true
            };
            // If creating an Informatics account, give full permissions
            if (Input.IsInformatics || string.Equals(user.Role, "Informatics", StringComparison.OrdinalIgnoreCase))
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

            try
            {
                _db.UserAccounts.Add(user);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = $"تم إنشاء الحساب {user.Username} بنجاح.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                var errorMessage = "حدث خطأ أثناء إنشاء الحساب: " + ex.Message;
                ModelState.AddModelError(string.Empty, errorMessage);
                TempData["ErrorMessage"] = errorMessage;
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int id)
        {
            var user = await _db.UserAccounts.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent disabling the current informatics account by itself accidentally
            var currentIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentIdClaim != null && user.Id.ToString() == currentIdClaim)
            {
                ModelState.AddModelError(string.Empty, "لا يمكن تغيير حالة الحساب الحالي.");
                await OnGetAsync();
                return Page();
            }

            user.IsActive = !user.IsActive;
            user.FailedLoginAttempts = 0;
            user.LockoutEndUtc = null;
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id,
            bool CanViewEmployees = false,
            bool CanAddEmployees = false,
            bool CanEditEmployees = false,
            bool CanDeleteEmployees = false,
            bool CanAddLeaves = false,
            bool CanEditLeaves = false,
            bool CanDeleteLeaves = false,
            bool CanManageAttachments = false,
            bool CanViewAuditLogs = false,
            bool CanEditAuditLogs = false,
            string DisplayName = "",
            string Role = "")
        {
            var user = await _db.UserAccounts.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent editing the current informatics account's role/activation accidentally
            var currentIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentIdClaim != null && user.Id.ToString() == currentIdClaim && !string.Equals(Role, user.Role, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "لا يمكن تغيير دور الحساب الحالي.");
                await OnGetAsync();
                return Page();
            }

            user.DisplayName = string.IsNullOrWhiteSpace(DisplayName) ? user.DisplayName : DisplayName;
            user.Role = string.IsNullOrWhiteSpace(Role) ? user.Role : Role;
            user.CanViewEmployees = CanViewEmployees;
            user.CanAddEmployees = CanAddEmployees;
            user.CanEditEmployees = CanEditEmployees;
            user.CanDeleteEmployees = CanDeleteEmployees;
            user.CanAddLeaves = CanAddLeaves;
            user.CanEditLeaves = CanEditLeaves;
            user.CanDeleteLeaves = CanDeleteLeaves;
            user.CanManageAttachments = CanManageAttachments;
            user.CanViewAuditLogs = CanViewAuditLogs;
            user.CanEditAuditLogs = CanEditAuditLogs;

            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSetInformaticsAsync(int id, bool setInformatics)
        {
            var user = await _db.UserAccounts.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentIdClaim != null && user.Id.ToString() == currentIdClaim && !setInformatics)
            {
                ModelState.AddModelError(string.Empty, "لا يمكن إزالة دور المعلوماتية من الحساب الحالي.");
                await OnGetAsync();
                return Page();
            }

            if (setInformatics)
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
                // demote to default viewer role and remove elevated permissions
                user.Role = "Viewer";
                user.CanViewEmployees = true;
                user.CanAddEmployees = false;
                user.CanEditEmployees = false;
                user.CanDeleteEmployees = false;
                user.CanAddLeaves = false;
                user.CanEditLeaves = false;
                user.CanDeleteLeaves = false;
                user.CanManageAttachments = false;
                user.CanViewAuditLogs = false;
                user.CanEditAuditLogs = false;
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = setInformatics ? "تم منح صلاحية المعلوماتية بنجاح." : "تم إزالة صلاحية المعلوماتية بنجاح.";
            return RedirectToPage();
        }
    }
}
