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

        // The currently selected character (slot 1–4)
        public CapstoneProject.Models.CharacterModel CurrentCharacter { get; set; }

        // Controls whether the textarea is shown
        [BindProperty]
        public bool IsEditingBio { get; set; }

        // Bio text
        [BindProperty]
        public string NewDescription { get; set; }

        // ⭐ NEW: Profile image upload
        [BindProperty]
        public IFormFile ProfileImage { get; set; }

        // Load user + character
        public IActionResult OnGet(int slot = 1)
        {
            var loggedIn = HttpContext.Session.GetString("LoggedIn");
            var username = HttpContext.Session.GetString("Username");

            if (loggedIn != "true")
                return RedirectToPage("/Index");

            CurrentUser = _userAccess.GetUserByUsername(username);

            if (CurrentUser == null)
                return RedirectToPage("/Index");

            CurrentCharacter = _characterAccess.GetCharacterBySlot(CurrentUser.User_ID.ToString(), slot);

            return Page();
        }

        // When Edit button is clicked
        public IActionResult OnPostEditBio()
        {
            var username = HttpContext.Session.GetString("Username");
            CurrentUser = _userAccess.GetUserByUsername(username);

            NewDescription = CurrentUser.User_Description;
            IsEditingBio = true;

            return Page();
        }

        // OLD save bio handler (kept intact)
        public IActionResult OnPostSaveBio()
        {
            var username = HttpContext.Session.GetString("Username");
            CurrentUser = _userAccess.GetUserByUsername(username);

            CurrentUser.User_Description = NewDescription;
            _userAccess.UpdateBio(CurrentUser);

            return RedirectToPage();
        }

        // ⭐ NEW: Unified handler for saving BOTH bio + profile picture
        public async Task<IActionResult> OnPostSaveAll()
        {
            var username = HttpContext.Session.GetString("Username");
            CurrentUser = _userAccess.GetUserByUsername(username);

            // 1. Save bio
            if (!string.IsNullOrWhiteSpace(NewDescription))
            {
                CurrentUser.User_Description = NewDescription;
                _userAccess.UpdateBio(CurrentUser);
            }

            // 2. Save profile picture if uploaded
            if (ProfileImage != null)
            {
                var uploadFolder = Path.Combine("wwwroot", "images", "users");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                var relativePath = $"/images/users/{fileName}";

                CurrentUser.Profilepic = relativePath;
                _userAccess.UpdateUserImage(CurrentUser);
            }

            return RedirectToPage();
        }
        public IActionResult OnPostDeleteCharacter(int slot)
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _userAccess.GetUserByUsername(username);

            _characterAccess.DeleteCharacter(user.User_ID, slot);

            return RedirectToPage(new { slot = slot });
        }

    }
}