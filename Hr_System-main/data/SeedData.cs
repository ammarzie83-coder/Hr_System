using Hr_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Hr_System.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Database.EnsureCreated();

            // Seed data should be idempotent: only add items that don't already exist.

            var imageBytes = Convert.FromBase64String(
                "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAEElEQVR42mP8z/C/HwAE/wJ+xcJONQAAAABJRU5ErkJggg==");

            var dummyPdf = Encoding.ASCII.GetBytes(
                "%PDF-1.4\n1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 200 200] /Contents 4 0 R >>\nendobj\n4 0 obj\n<< /Length 64 >>\nstream\nBT /F1 24 Tf 50 100 Td (Sample PDF Attachment) Tj ET\nendstream\nendobj\ntrailer\n<< /Root 1 0 R >>\n%%EOF");

            var employees = new List<Employee>
            {
                // new Employee
                // {
                //     FullName = "علي محمد",
                //     NationalId = "1234567890",
                //     JobTitle = "أخصائي تنمية بشرية",
                //     Department = "قسم التنمية",
                //     Mobile = "+966500123456",
                //     Email = "ali.mohammed@hrsystem.local",
                //     HireDate = new DateTime(2022, 1, 15),
                //     PhotoData = imageBytes,
                //     PhotoContentType = "image/png"
                // },
                // new Employee
                // {
                //     FullName = "سارة أحمد",
                //     NationalId = "0987654321",
                //     JobTitle = "مديرة تدريب",
                //     Department = "قسم التنمية",
                //     Mobile = "+966501234567",
                //     Email = "sarah.ahmed@hrsystem.local",
                //     HireDate = new DateTime(2021, 3, 8),
                //     PhotoData = imageBytes,
                //     PhotoContentType = "image/png"
                // },
                // new Employee
                // {
                //     FullName = "محمد العتيبي",
                //     NationalId = "1122334455",
                //     JobTitle = "محلل أنظمة",
                //     Department = "قسم التنمية",
                //     Mobile = "+966502345678",
                //     Email = "mohammed.otaibi@hrsystem.local",
                //     HireDate = new DateTime(2023, 5, 20),
                //     PhotoData = imageBytes,
                //     PhotoContentType = "image/png"
                // },
                // new Employee
                // {
                //     FullName = "ليلى حسن",
                //     NationalId = "5566778899",
                //     JobTitle = "إداري موارد بشرية",
                //     Department = "قسم التنمية",
                //     Mobile = "+966503456789",
                //     Email = "leila.hassan@hrsystem.local",
                //     HireDate = new DateTime(2020, 11, 1),
                //     PhotoData = imageBytes,
                //     PhotoContentType = "image/png"
                // }
                // ,
                // new Employee
                // {
                //     FullName = "موظف المعلوماتية",
                //     NationalId = "0000000001",
                //     JobTitle = "مهندس نظم معلومات",
                //     Department = "قسم المعلوماتية",
                //     Mobile = "+966509999999",
                //     Email = "informatics@hrsystem.local",
                //     HireDate = new DateTime(2024, 5, 20),
                //     PhotoData = imageBytes,
                //     PhotoContentType = "image/png"
                // }
            };

            var attachments = new List<EmployeeAttachment>
            {
                // new EmployeeAttachment
                // {
                //     Employee = employees[0],
                //     FileName = "شهادة-خبرة.pdf",
                //     ContentType = "application/pdf",
                //     Data = dummyPdf
                // },
                // new EmployeeAttachment
                // {
                //     Employee = employees[1],
                //     FileName = "شهادة-دورات.pdf",
                //     ContentType = "application/pdf",
                //     Data = dummyPdf
                // }
            };

            var leaves = new List<LeaveRequest>
            {
                // new LeaveRequest
                // {
                //     Employee = employees[0],
                //     LeaveType = "إجازة سنوية",
                //     StartDate = new DateTime(2024, 6, 1),
                //     EndDate = new DateTime(2024, 6, 10),
                //     Reason = "تمديد إجازة سنوية بعد التنسيق.",
                //     Status = "Approved"
                // },
                // new LeaveRequest
                // {
                //     Employee = employees[1],
                //     LeaveType = "إجازة مرضية",
                //     StartDate = new DateTime(2024, 7, 5),
                //     EndDate = new DateTime(2024, 7, 8),
                //     Reason = "متابعة طبية.",
                //     Status = "Pending"
                // }
            };

            var users = new List<UserAccount>
            {
                // new UserAccount
                // {
                //     Username = "admin",
                //     PasswordHash = PasswordHelper.HashPassword("Admin2026!"),
                //     DisplayName = "المدير الرئيسي",
                //     Role = "Administrator",
                //     CanViewEmployees = true,
                //     CanAddEmployees = true,
                //     CanEditEmployees = true,
                //     CanDeleteEmployees = true,
                //     CanAddLeaves = true,
                //     CanEditLeaves = true,
                //     CanDeleteLeaves = true,
                //     CanManageAttachments = true
                // },
                // new UserAccount
                // {
                //     Username = "devadmin",
                //     PasswordHash = PasswordHelper.HashPassword("P@ssw0rd123!"),
                //     DisplayName = "مدير قسم التنمية",
                //     Role = "DevelopmentManager",
                //     CanViewEmployees = true,
                //     CanAddEmployees = true,
                //     CanEditEmployees = true,
                //     CanDeleteEmployees = true,
                //     CanAddLeaves = true,
                //     CanEditLeaves = true,
                //     CanDeleteLeaves = true,
                //     CanManageAttachments = true
                // },
                // new UserAccount
                // {
                //     Username = "devassistant",
                //     PasswordHash = PasswordHelper.HashPassword("Employee2026!"),
                //     DisplayName = "مساعد تنمية",
                //     Role = "DevelopmentAssistant",
                //     CanViewEmployees = true,
                //     CanAddEmployees = true,
                //     CanEditEmployees = false,
                //     CanDeleteEmployees = true,
                //     CanAddLeaves = true,
                //     CanEditLeaves = false,
                //     CanDeleteLeaves = true,
                //     CanManageAttachments = true
                // }
                // ,
                // new UserAccount
                // {
                //     Username = "informatics",
                //     PasswordHash = PasswordHelper.HashPassword("Informatics2026!"),
                //     DisplayName = "قسم المعلوماتية",
                //     Role = "Informatics",
                //     CanViewEmployees = true,
                //     CanAddEmployees = true,
                //     CanEditEmployees = true,
                //     CanDeleteEmployees = true,
                //     CanAddLeaves = true,
                //     CanEditLeaves = true,
                //     CanDeleteLeaves = true,
                //     CanManageAttachments = true
                //     ,CanViewAuditLogs = true
                //     ,CanEditAuditLogs = true
                // },
                // new UserAccount
                // {
                //     Username = "demo_viewer",
                //     PasswordHash = PasswordHelper.HashPassword("Viewer2026!"),
                //     DisplayName = "مستخدم عرض",
                //     Role = "Viewer",
                //     CanViewEmployees = true,
                //     CanManageAttachments = true
                // }
            };

            // Add users if missing
            foreach (var u in users)
            {
                if (!context.UserAccounts.Any(x => x.Username == u.Username))
                {
                    context.UserAccounts.Add(u);
                }
            }

            // Add employee records for email-style user accounts that are not yet represented in Employees.
            var emailUserAccounts = context.UserAccounts
                .Where(u => u.Username.Contains("@"))
                .ToList();


            // Add employees if missing (match by Email)
            foreach (var e in employees)
            {
                if (!context.Employees.Any(x => x.Email == e.Email))
                {
                    context.Employees.Add(e);
                    if (e.Email == "informatics@hrsystem.local")
                    {
                        Console.WriteLine($"✓ Added informatics employee: {e.FullName} ({e.Email})");
                    }
                }
            }

            // Add attachments if missing (match by FileName); attach to existing employee when possible
            foreach (var a in attachments)
            {
                if (!context.EmployeeAttachments.Any(x => x.FileName == a.FileName))
                {
                    if (a.Employee != null && !string.IsNullOrEmpty(a.Employee.Email))
                    {
                        var emp = context.Employees.FirstOrDefault(x => x.Email == a.Employee.Email);
                        if (emp != null)
                        {
                            a.EmployeeId = emp.Id;
                            a.Employee = null;
                        }
                    }

                    context.EmployeeAttachments.Add(a);
                }
            }

            // Add leaves if missing (match by EmployeeId + StartDate)
            foreach (var l in leaves)
            {
                var empEmail = l.Employee?.Email;
                if (!string.IsNullOrEmpty(empEmail))
                {
                    var emp = context.Employees.FirstOrDefault(x => x.Email == empEmail);
                    if (emp != null)
                    {
                        if (!context.LeaveRequests.Any(x => x.EmployeeId == emp.Id && x.StartDate == l.StartDate))
                        {
                            l.EmployeeId = emp.Id;
                            l.Employee = null;
                            context.LeaveRequests.Add(l);
                        }
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
