using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CapstoneProject.Models;

namespace CapstoneProject.Pages
{
    public class Index1Model : PageModel
    {
        private readonly UserAccessLayer _userAccess;

        public Index1Model(UserAccessLayer userAccess)
        {
            _userAccess = userAccess;
        }

        public UserModel CurrentUser { get; set; }

        public IActionResult OnGet()
        {
            var loggedIn = HttpContext.Session.GetString("LoggedIn");
            var username = HttpContext.Session.GetString("Username");

            if (loggedIn != "true")
                return RedirectToPage("/Index");

            CurrentUser = _userAccess.GetUserByUsername(username);

            if (CurrentUser == null)
                return RedirectToPage("/Index");

            return Page();
        }
    }
}