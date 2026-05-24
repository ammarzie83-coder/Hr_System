using Hr_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.Security.Claims;

namespace Hr_System.Pages.Employees
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public List<EmployeeViewItem> Employees { get; set; } = new();

        public bool CanAddEmployees => User.HasPermission(PermissionConstants.EmployeesCreate);
        public bool CanEditEmployees => User.HasPermission(PermissionConstants.EmployeesEdit);
        public bool CanDeleteEmployees => User.HasPermission(PermissionConstants.EmployeesDelete);

        public async Task OnGetAsync()
        {
            Employees = await _db.Employees
                .AsNoTracking()
                .Select(e => new EmployeeViewItem
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    NationalId = e.NationalId,
                    JobTitle = e.JobTitle,
                    Department = e.Department,
                    Mobile = e.Mobile,
                    Email = e.Email,
                    HireDate = e.HireDate,
                    LeaveCount = e.LeaveRequests.Count,
                    AttachmentCount = e.Attachments.Count
                })
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetExportAsync()
        {
            var employees = await _db.Employees
                .OrderBy(e => e.FullName)
                .Select(e => new
                {
                    e.FullName,
                    e.NationalId,
                    e.JobTitle,
                    e.Department,
                    e.Mobile,
                    e.Email,
                    e.HireDate
                })
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("الموظفون");
            worksheet.Cell(1, 1).Value = "الاسم";
            worksheet.Cell(1, 2).Value = "الرقم الوطني";
            worksheet.Cell(1, 3).Value = "الوظيفة";
            worksheet.Cell(1, 4).Value = "القسم";
            worksheet.Cell(1, 5).Value = "الهاتف";
            worksheet.Cell(1, 6).Value = "البريد";
            worksheet.Cell(1, 7).Value = "تاريخ التعيين";

            for (int index = 0; index < employees.Count; index++)
            {
                var row = index + 2;
                var item = employees[index];
                worksheet.Cell(row, 1).Value = item.FullName;
                worksheet.Cell(row, 2).Value = item.NationalId;
                worksheet.Cell(row, 3).Value = item.JobTitle;
                worksheet.Cell(row, 4).Value = item.Department;
                worksheet.Cell(row, 5).Value = item.Mobile;
                worksheet.Cell(row, 6).Value = item.Email;
                worksheet.Cell(row, 7).Value = item.HireDate.ToString("yyyy-MM-dd");
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "HrEmployees.xlsx");
        }

        public class EmployeeViewItem
        {
            public int Id { get; set; }
            public string FullName { get; set; } = null!;
            public string NationalId { get; set; } = null!;
            public string JobTitle { get; set; } = null!;
            public string Department { get; set; } = null!;
            public string Mobile { get; set; } = null!;
            public string Email { get; set; } = null!;
            public DateTime HireDate { get; set; }
            public int LeaveCount { get; set; }
            public int AttachmentCount { get; set; }
        }
    }
}
