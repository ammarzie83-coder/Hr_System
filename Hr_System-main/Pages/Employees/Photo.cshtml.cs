using Hr_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hr_System.Pages.Employees
{
    [Authorize]
    public class PhotoModel : PageModel
    {
        private readonly AppDbContext _db;

        public PhotoModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var employee = await _db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null || employee.PhotoData == null)
            {
                return NotFound();
            }

            return File(employee.PhotoData, employee.PhotoContentType ?? "image/png");
        }
    }
}
