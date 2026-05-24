using Hr_System.Data;
using Hr_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hr_System.Pages.Audit
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public List<AuditLog> Logs { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.HasPermission(PermissionConstants.AuditView))
            {
                return Forbid();
            }

            Logs = await _db.AuditLogs
                .AsNoTracking()
                .OrderByDescending(a => a.ChangedAt)
                .Take(500)
                .ToListAsync();

            return Page();
        }
    }
}
