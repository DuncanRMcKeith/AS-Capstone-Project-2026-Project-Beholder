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

        // NEW: Only the editable fields
        [BindProperty]
        public EditUserModel EditUser { get; set; }

        // NEW: Controls whether the textarea is shown
        [BindProperty]
        public bool IsEditingBio { get; set; }

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

        // NEW: When Edit button is clicked
        public IActionResult OnPostEditBio()
        {
            var username = HttpContext.Session.GetString("Username");
            CurrentUser = _userAccess.GetUserByUsername(username);

            // Pre-fill the edit model with the current description
            EditUser = new EditUserModel
            {
                User_Description = CurrentUser.User_Description
            };

            IsEditingBio = true;
            return Page();
        }

        // NEW: When Save button is clicked
        public IActionResult OnPostSaveBio()
        {
            var username = HttpContext.Session.GetString("Username");
            CurrentUser = _userAccess.GetUserByUsername(username);

            // Save ONLY the description — no ID editing
            _userAccess.UpdateBio(new UserModel
            {
                User_ID = CurrentUser.User_ID,
                User_Description = EditUser.User_Description
            });

            return RedirectToPage();
        }
    }
}