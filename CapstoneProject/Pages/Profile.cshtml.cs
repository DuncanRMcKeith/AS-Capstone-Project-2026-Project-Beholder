using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CapstoneProject.Models;

namespace CapstoneProject.Pages
{
    public class Index1Model : PageModel
    {
        private readonly UserAccessLayer _userAccess;
        private readonly CharacterAccessLayer _characterAccess;

        public Index1Model(UserAccessLayer userAccess, CharacterAccessLayer characterAccess)
        {
            _userAccess = userAccess;
            _characterAccess = characterAccess;
        }

        public UserModel CurrentUser { get; set; }

        // NEW: The currently selected character (slot 1–4)
        public CapstoneProject.Models.CharacterModel CurrentCharacter { get; set; }

        // NEW: Controls whether the textarea is shown
        [BindProperty]
        public bool IsEditingBio { get; set; }

        // NEW: BindProperty for NewDescription
        [BindProperty]
        public string NewDescription { get; set; }

        // UPDATED: Now accepts ?slot=1,2,3,4
        public IActionResult OnGet(int slot = 1)
        {
            var loggedIn = HttpContext.Session.GetString("LoggedIn");
            var username = HttpContext.Session.GetString("Username");

            if (loggedIn != "true")
                return RedirectToPage("/Index");

            CurrentUser = _userAccess.GetUserByUsername(username);

            if (CurrentUser == null)
                return RedirectToPage("/Index");

            // NEW: Load the selected character
            CurrentCharacter = _characterAccess.GetCharacterBySlot(CurrentUser.User_ID.ToString(), slot);

            return Page();
        }

        // NEW: When Edit button is clicked
        public IActionResult OnPostEditBio()
        {
            var username = HttpContext.Session.GetString("Username");
            CurrentUser = _userAccess.GetUserByUsername(username);

            NewDescription = CurrentUser.User_Description;
            IsEditingBio = true;

            return Page();
        }

        public IActionResult OnPostSaveBio()
        {
            var username = HttpContext.Session.GetString("Username");
            CurrentUser = _userAccess.GetUserByUsername(username);

            CurrentUser.User_Description = NewDescription;
            _userAccess.UpdateBio(CurrentUser);

            return RedirectToPage();
        }
    }
}