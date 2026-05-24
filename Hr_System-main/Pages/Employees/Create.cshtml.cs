using Hr_System.Data;
using Hr_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hr_System.Pages.Employees
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;

        public CreateModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public EmployeeInputModel Input { get; set; } = new EmployeeInputModel();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.HasPermission(PermissionConstants.EmployeesCreate))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var employee = new Employee
            {
                FullName = Input.FullName,
                NationalId = Input.NationalId,
                JobTitle = Input.JobTitle,
                Department = Input.Department,
                Mobile = Input.Mobile,
                Email = Input.Email,
                HireDate = Input.HireDate
            };

            if (Input.PhotoUpload != null)
            {
                if (!Input.PhotoUpload.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("Input.PhotoUpload", "يجب أن تكون الصورة بصيغة PNG أو JPEG.");
                    return Page();
                }

                await using var ms = new MemoryStream();
                await Input.PhotoUpload.CopyToAsync(ms);
                employee.PhotoData = ms.ToArray();
                employee.PhotoContentType = Input.PhotoUpload.ContentType;
            }

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم إنشاء الموظف بنجاح.";
            return RedirectToPage("Index");
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
    }
}
