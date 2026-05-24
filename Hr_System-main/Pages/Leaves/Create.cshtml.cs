using Hr_System.Data;
using Hr_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hr_System.Pages.Leaves
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;

        public CreateModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int EmployeeId { get; set; }

        [BindProperty]
        public LeaveInputModel Input { get; set; } = new LeaveInputModel();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.HasPermission(PermissionConstants.LeavesCreate))
            {
                return Forbid();
            }

            var employeeExists = await _db.Employees.AnyAsync(e => e.Id == EmployeeId);
            if (!employeeExists)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.HasPermission(PermissionConstants.LeavesCreate))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!await _db.Employees.AnyAsync(e => e.Id == EmployeeId))
            {
                return NotFound();
            }

            var leave = new LeaveRequest
            {
                EmployeeId = EmployeeId,
                LeaveType = Input.LeaveType,
                StartDate = Input.StartDate,
                EndDate = Input.EndDate,
                Reason = Input.Reason,
                Status = Input.Status
            };

            _db.LeaveRequests.Add(leave);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم تقديم طلب الإجازة بنجاح.";
            return RedirectToPage("../Employees/Details", new { id = EmployeeId });
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
