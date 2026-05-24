using Hr_System.Data;
using Hr_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hr_System.Pages.Employees
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditModel(AppDbContext db)
        {
            _db = db;
        }

        public bool CanManageAttachments => User.HasPermission(PermissionConstants.AttachmentsManage);
        public bool CanAddLeaves => User.HasPermission(PermissionConstants.LeavesCreate);
            public bool CanEditLeaves => User.HasPermission(PermissionConstants.LeavesEdit);
            public bool CanDeleteLeaves => User.HasPermission(PermissionConstants.LeavesDelete);

            public List<AttachmentItem> Attachments { get; set; } = new();
            public List<LeaveItem> Leaves { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public EmployeeInputModel Input { get; set; } = new EmployeeInputModel();

        [BindProperty]
        public IFormFile? AttachmentUpload { get; set; }

        [BindProperty]
        public LeaveInputModel LeaveInput { get; set; } = new LeaveInputModel();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.HasPermission(PermissionConstants.EmployeesEdit))
            {
                return Forbid();
            }

            var employee = await _db.Employees.FindAsync(Id);
            if (employee == null)
            {
                return NotFound();
            }

            Input.FullName = employee.FullName;
            Input.NationalId = employee.NationalId;
            Input.JobTitle = employee.JobTitle;
            Input.Department = employee.Department;
            Input.Mobile = employee.Mobile;
            Input.Email = employee.Email;
            Input.HireDate = employee.HireDate;

            await LoadRelatedDataAsync();
            return Page();
        }

        private async Task LoadRelatedDataAsync()
        {
            Attachments = await _db.EmployeeAttachments
                .Where(a => a.EmployeeId == Id)
                .Select(a => new AttachmentItem { Id = a.Id, FileName = a.FileName })
                .ToListAsync();

            Leaves = await _db.LeaveRequests
                .Where(l => l.EmployeeId == Id)
                .Select(l => new LeaveItem { Id = l.Id, LeaveType = l.LeaveType, StartDate = l.StartDate, EndDate = l.EndDate, Reason = l.Reason, Status = l.Status })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.HasPermission(PermissionConstants.EmployeesEdit))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await LoadRelatedDataAsync();
                return Page();
            }

            var employee = await _db.Employees.FindAsync(Id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.FullName = Input.FullName;
            employee.NationalId = Input.NationalId;
            employee.JobTitle = Input.JobTitle;
            employee.Department = Input.Department;
            employee.Mobile = Input.Mobile;
            employee.Email = Input.Email;
            employee.HireDate = Input.HireDate;

            if (Input.PhotoUpload != null)
            {
                if (!Input.PhotoUpload.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("Input.PhotoUpload", "يجب أن تكون الصورة بصيغة PNG أو JPEG.");
                    await LoadRelatedDataAsync();
                    return Page();
                }

                await using var ms = new MemoryStream();
                await Input.PhotoUpload.CopyToAsync(ms);
                employee.PhotoData = ms.ToArray();
                employee.PhotoContentType = Input.PhotoUpload.ContentType;
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم حفظ بيانات الموظف بنجاح.";
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostAddAttachmentAsync()
        {
            if (!User.HasPermission(PermissionConstants.AttachmentsManage))
            {
                return Forbid();
            }

            var employee = await _db.Employees.FindAsync(Id);
            if (employee == null)
            {
                return NotFound();
            }

            if (AttachmentUpload == null)
            {
                ModelState.AddModelError("AttachmentUpload", "يرجى اختيار ملف PDF.");
                await LoadRelatedDataAsync();
                return Page();
            }

            if (AttachmentUpload.ContentType != "application/pdf")
            {
                ModelState.AddModelError("AttachmentUpload", "يجب أن يكون الملف بصيغة PDF.");
                await LoadRelatedDataAsync();
                return Page();
            }

            await using var ms = new MemoryStream();
            await AttachmentUpload.CopyToAsync(ms);

            var attachment = new EmployeeAttachment
            {
                EmployeeId = Id,
                FileName = AttachmentUpload.FileName,
                ContentType = AttachmentUpload.ContentType,
                Data = ms.ToArray()
            };

            _db.EmployeeAttachments.Add(attachment);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم إضافة المرفق بنجاح.";
            return RedirectToPage(new { id = Id });
        }

        public async Task<IActionResult> OnPostAddLeaveAsync()
        {
            if (!User.HasPermission(PermissionConstants.LeavesCreate))
            {
                return Forbid();
            }

            var employee = await _db.Employees.FindAsync(Id);
            if (employee == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadRelatedDataAsync();
                return Page();
            }

            var leave = new LeaveRequest
            {
                EmployeeId = Id,
                LeaveType = LeaveInput.LeaveType,
                StartDate = LeaveInput.StartDate,
                EndDate = LeaveInput.EndDate,
                Reason = LeaveInput.Reason,
                Status = "Pending"
            };

            _db.LeaveRequests.Add(leave);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم تقديم طلب الإجازة بنجاح.";
            return RedirectToPage(new { id = Id });
        }

        public async Task<IActionResult> OnPostDeleteAttachmentAsync(int attachmentId)
        {
            if (!User.HasPermission(PermissionConstants.AttachmentsManage))
            {
                return Forbid();
            }

            var attachment = await _db.EmployeeAttachments.FindAsync(attachmentId);
            if (attachment == null || attachment.EmployeeId != Id)
            {
                return NotFound();
            }

            _db.EmployeeAttachments.Remove(attachment);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم حذف المرفق بنجاح.";
            return RedirectToPage(new { id = Id });
        }

        public async Task<IActionResult> OnPostDeleteLeaveAsync(int leaveId)
        {
            if (!User.HasPermission(PermissionConstants.LeavesDelete))
            {
                return Forbid();
            }

            var leave = await _db.LeaveRequests.FindAsync(leaveId);
            if (leave == null || leave.EmployeeId != Id)
            {
                return NotFound();
            }

            _db.LeaveRequests.Remove(leave);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم حذف طلب الإجازة بنجاح.";
            return RedirectToPage(new { id = Id });
        }

        public class EmployeeInputModel
        {
            public string FullName { get; set; } = string.Empty;
            public string NationalId { get; set; } = string.Empty;
            public string JobTitle { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string Mobile { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public DateTime HireDate { get; set; } = DateTime.UtcNow;
            public IFormFile? PhotoUpload { get; set; }
        }

        public class LeaveInputModel
        {
            public string LeaveType { get; set; } = string.Empty;
            public DateTime StartDate { get; set; } = DateTime.UtcNow;
            public DateTime EndDate { get; set; } = DateTime.UtcNow;
            public string Reason { get; set; } = string.Empty;
        }

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
