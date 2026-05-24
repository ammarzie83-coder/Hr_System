using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hr_System.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string reason = "")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // حفظ رسالة سبب الخروج
            if (!string.IsNullOrEmpty(reason))
            {
                // سبب الخروج (مثل انتهاء الجلسة) يُعامل كرسالة خطأ/تنبيه
                TempData["SessionExpiredMessage"] = reason;
            }
            else
            {
                // خروج يدوي: عرض رسالة نجاح باللون الأخضر
                TempData["SuccessMessage"] = "تم تسجيل الخروج بنجاح.";
            }

            // مسح آخر صفحة من السشن
            HttpContext.Session.Remove("LastPage");

            return RedirectToPage("/Account/Login");
        }
    }
}
