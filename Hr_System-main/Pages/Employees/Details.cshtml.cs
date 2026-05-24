using Hr_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hr_System.Pages.Employees
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _db;

        public DetailsModel(AppDbContext db)
        {
            _db = db;
        }

        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }

        public List<AttachmentItem> Attachments { get; set; } = new();
        public List<LeaveItem> Leaves { get; set; } = new();

        // Upload moved to Edit page

        public bool CanManageAttachments => User.HasPermission(PermissionConstants.AttachmentsManage);
        public bool CanAddLeaves => User.HasPermission(PermissionConstants.LeavesCreate);
        public bool CanEditLeaves => User.HasPermission(PermissionConstants.LeavesEdit);
        public bool CanDeleteLeaves => User.HasPermission(PermissionConstants.LeavesDelete);

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!User.HasPermission(PermissionConstants.EmployeesView))
            {
                return Forbid();
            }

            var employee = await _db.Employees
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    e.FullName,
                    e.JobTitle,
                    e.Department,
                    e.Mobile,
                    e.Email,
                    e.HireDate,
                    Attachments = e.Attachments.Select(a => new AttachmentItem
                    {
                        Id = a.Id,
                        FileName = a.FileName
                    }).ToList(),
                    Leaves = e.LeaveRequests.Select(l => new LeaveItem
                    {
                        Id = l.Id,
                        LeaveType = l.LeaveType,
                        StartDate = l.StartDate,
                        EndDate = l.EndDate,
                        Reason = l.Reason,
                        Status = l.Status
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            EmployeeId = employee.Id;
            FullName = employee.FullName;
            JobTitle = employee.JobTitle;
            Department = employee.Department;
            Mobile = employee.Mobile;
            Email = employee.Email;
            HireDate = employee.HireDate;
            Attachments = employee.Attachments;
            Leaves = employee.Leaves;

            return Page();
        }

        // Attachment upload handler moved to Edit page (OnPostAddAttachmentAsync)

        // All mutation handlers moved to Edit page. Details is view-only.

        public class AttachmentItem
        {
            public int Id { get; set; }
            public string FileName { get; set; } = string.Empty;
        }

        public class LeaveItem
        {
            public int Id { get; set; }
            public string LeaveType { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Reason { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }
    }
}
