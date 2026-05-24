using Hr_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hr_System.Pages.Leaves
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

        public int EmployeeId { get; set; }
        public string LeaveType { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.HasPermission(PermissionConstants.LeavesDelete))
            {
                return Forbid();
            }

            var leave = await _db.LeaveRequests.FindAsync(Id);
            if (leave == null)
            {
                return NotFound();
            }

            EmployeeId = leave.EmployeeId;
            LeaveType = leave.LeaveType;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.HasPermission(PermissionConstants.LeavesDelete))
            {
                return Forbid();
            }

            var leave = await _db.LeaveRequests.FindAsync(Id);
            if (leave == null)
            {
                return NotFound();
            }

            var employeeId = leave.EmployeeId;
            _db.LeaveRequests.Remove(leave);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم حذف طلب الإجازة بنجاح.";
            return RedirectToPage("../Employees/Details", new { id = employeeId });
        }
    }
}
