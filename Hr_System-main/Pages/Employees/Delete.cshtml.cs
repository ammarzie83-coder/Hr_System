using Hr_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hr_System.Pages.Employees
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _db;

        public DeleteModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.HasPermission(PermissionConstants.EmployeesDelete))
            {
                return Forbid();
            }

            var employee = await _db.Employees.FindAsync(Id);
            if (employee == null)
            {
                return NotFound();
            }

            FullName = employee.FullName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.HasPermission(PermissionConstants.EmployeesDelete))
            {
                return Forbid();
            }

            var employee = await _db.Employees.FindAsync(Id);
            if (employee == null)
            {
                return NotFound();
            }

            var attachments = _db.EmployeeAttachments.Where(a => a.EmployeeId == Id);
            _db.EmployeeAttachments.RemoveRange(attachments);

            var leaves = _db.LeaveRequests.Where(l => l.EmployeeId == Id);
            _db.LeaveRequests.RemoveRange(leaves);

            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم حذف الموظف بنجاح.";
            return RedirectToPage("Index");
        }
    }
}
