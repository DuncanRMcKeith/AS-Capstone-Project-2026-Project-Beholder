using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            TempData["ShowLogoutToast"] = true;
            return RedirectToPage("/Login");
        }
    }
}
