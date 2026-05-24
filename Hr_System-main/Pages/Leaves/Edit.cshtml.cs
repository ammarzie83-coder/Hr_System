using Hr_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hr_System.Pages.Leaves
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [BindProperty]
        public LeaveInputModel Input { get; set; } = new LeaveInputModel();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.HasPermission(PermissionConstants.LeavesEdit))
            {
                return Forbid();
            }

            var leave = await _db.LeaveRequests.FindAsync(Id);
            if (leave == null)
            {
                return NotFound();
            }

            EmployeeId = leave.EmployeeId;
            Input.LeaveType = leave.LeaveType;
            Input.StartDate = leave.StartDate;
            Input.EndDate = leave.EndDate;
            Input.Reason = leave.Reason;
            Input.Status = leave.Status;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.HasPermission(PermissionConstants.LeavesEdit))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var leave = await _db.LeaveRequests.FindAsync(Id);
            if (leave == null)
            {
                return NotFound();
            }

            leave.LeaveType = Input.LeaveType;
            leave.StartDate = Input.StartDate;
            leave.EndDate = Input.EndDate;
            leave.Reason = Input.Reason;
            leave.Status = Input.Status;

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم تحديث طلب الإجازة بنجاح.";
            return RedirectToPage("../Employees/Details", new { id = leave.EmployeeId });
        }

        public class LeaveInputModel
        {
            public string LeaveType { get; set; } = string.Empty;
            public DateTime StartDate { get; set; } = DateTime.UtcNow;
            public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(1);
            public string Reason { get; set; } = string.Empty;
            public string Status { get; set; } = "Pending";
        }
    }
}
