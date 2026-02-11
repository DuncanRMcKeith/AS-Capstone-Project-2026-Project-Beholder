using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class Index1Model : PageModel
    {
        public IActionResult OnGet()
        {
            var loggedIn = HttpContext.Session.GetString("LoggedIn");

            if (loggedIn != "true")
                return RedirectToPage("/Index");

            return Page();
        }
    }
}
