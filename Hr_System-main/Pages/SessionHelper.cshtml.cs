using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hr_System.Pages
{
    [IgnoreAntiforgeryToken]
    public class SessionHelperModel : PageModel
    {
        public IActionResult OnPostSaveLastPage([FromBody] SaveLastPageRequest request)
        {
            if (!string.IsNullOrEmpty(request?.LastPage))
            {
                HttpContext.Session.SetString("LastPage", request.LastPage);
                return new OkResult();
            }

            return new BadRequestResult();
        }
    }

    public class SaveLastPageRequest
    {
        public string LastPage { get; set; } = string.Empty;
    }
}
