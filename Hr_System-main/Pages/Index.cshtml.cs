using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hr_System.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // إذا كان المستخدم مسجل دخول
            if (User.Identity?.IsAuthenticated ?? false)
            {
                // محاولة الحصول على آخر صفحة تم زيارتها
                var lastPage = HttpContext.Session.GetString("LastPage");
                
                if (!string.IsNullOrEmpty(lastPage))
                {
                    // التوجيه لآخر صفحة
                    return Redirect(lastPage);
                }
                
                // إذا لم توجد آخر صفحة، التوجيه لصفحة الموظفين
                return RedirectToPage("/Employees/Index");
            }

            // إذا لم يكن مسجل دخول، التوجيه لصفحة اللوجن
            return RedirectToPage("/Account/Login");
        }
    }
}
