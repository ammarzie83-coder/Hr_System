using Hr_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace Hr_System.Pages.Employees
{
    [Authorize]
    public class DownloadAttachmentModel : PageModel
    {
        private readonly AppDbContext _db;

        public DownloadAttachmentModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!User.HasPermission(PermissionConstants.AttachmentsManage))
            {
                return Forbid();
            }

            var attachment = await _db.EmployeeAttachments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (attachment == null)
            {
                return NotFound();
            }

            // Properly encode filename for Content-Disposition header to handle non-ASCII characters
            var contentDisposition = new ContentDispositionHeaderValue("inline")
            {
                FileName = attachment.FileName
            };
            Response.Headers["Content-Disposition"] = contentDisposition.ToString();
            return File(attachment.Data, attachment.ContentType);
        }
    }
}
