using Hr_System.Data;
using Hr_System.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Hr_System.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;

        public LoginModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToPage("/Employees/Index");
            }

            ClearSessionAndSetNoCache();

            // تحقق من وجود رسائل من TempData: رسالة نجاح أو رسالة انتهاء/سبب الخروج
            if (TempData.TryGetValue("SuccessMessage", out var success))
            {
                SuccessMessage = success?.ToString() ?? string.Empty;
            }
            else if (TempData.TryGetValue("SessionExpiredMessage", out var message))
            {
                ErrorMessage = message?.ToString() ?? "انتهت مهلة جلسة العمل، يرجى تسجيل الدخول مجدداً.";
            }

            return Page();
        }

        private void ClearSessionAndSetNoCache()
        {
            HttpContext.Session.Clear();
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)
            {
                ErrorMessage = "يرجى ملء جميع الحقول المطلوبة.";
                if (isAjax) return new JsonResult(new { success = false, message = ErrorMessage });
                return Page();
            }

            // Find user by username (regardless of IsActive) to allow handling lockouts
            var user = await _db.UserAccounts.FirstOrDefaultAsync(u => u.Username == Username);
            if (user == null)
            {
                // Generic message to avoid user enumeration
                ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة.";
                if (isAjax) return new JsonResult(new { success = false, message = ErrorMessage });
                return Page();
            }

            if (!user.IsActive)
            {
                ErrorMessage = "الحساب غير مفعل، يرجى التواصل مع قسم المعلوماتية لإعادة التفعيل.";
                if (isAjax) return new JsonResult(new { success = false, message = ErrorMessage });
                return Page();
            }

            if (!PasswordHelper.VerifyPassword(Password, user.PasswordHash))
            {
                // Increment failed attempts and lock/deactivate after 5
                user.FailedLoginAttempts += 1;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.IsActive = false; // require manual re-activation by informatics
                    user.LockoutEndUtc = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                    ErrorMessage = "تم قفل الحساب مؤقتاً بعد محاولات كلمة مرور خاطئة متعددة. تواصل مع المعلوماتية لإعادة التفعيل.";
                    if (isAjax) return new JsonResult(new { success = false, message = ErrorMessage });
                    return Page();
                }

                await _db.SaveChangesAsync();
                ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة.";
                if (isAjax) return new JsonResult(new { success = false, message = ErrorMessage });
                return Page();
            }

            // Successful login: reset failed attempts
            user.FailedLoginAttempts = 0;
            user.LockoutEndUtc = null;
            await _db.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            AddPermissionClaims(user, claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true
                });

            SuccessMessage = $"مرحباً {user.DisplayName}، تم تسجيل الدخول بنجاح! 🎉";
            if (isAjax)
            {
                return new JsonResult(new { success = true, redirectUrl = Url.Page("/Employees/Index") });
            }

            return RedirectToPage("/Employees/Index");
        }

        private static void AddPermissionClaims(UserAccount user, List<Claim> claims)
        {
            if (user.CanViewEmployees)
            {
                claims.Add(new Claim("Permission", PermissionConstants.EmployeesView));
            }

            if (user.CanAddEmployees)
            {
                claims.Add(new Claim("Permission", PermissionConstants.EmployeesCreate));
            }

            if (user.CanEditEmployees)
            {
                claims.Add(new Claim("Permission", PermissionConstants.EmployeesEdit));
            }

            if (user.CanDeleteEmployees)
            {
                claims.Add(new Claim("Permission", PermissionConstants.EmployeesDelete));
            }

            if (user.CanAddLeaves)
            {
                claims.Add(new Claim("Permission", PermissionConstants.LeavesCreate));
            }

            if (user.CanEditLeaves)
            {
                claims.Add(new Claim("Permission", PermissionConstants.LeavesEdit));
            }

            if (user.CanDeleteLeaves)
            {
                claims.Add(new Claim("Permission", PermissionConstants.LeavesDelete));
            }

            if (user.CanManageAttachments)
            {
                claims.Add(new Claim("Permission", PermissionConstants.AttachmentsManage));
            }
            if (user.CanViewAuditLogs)
            {
                claims.Add(new Claim("Permission", PermissionConstants.AuditView));
            }

            if (user.CanEditAuditLogs)
            {
                claims.Add(new Claim("Permission", PermissionConstants.AuditEdit));
            }
        }
    }
}
