using CapstoneProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CapstoneProject.Pages
{
    public class CharacterInput : PageModel
    {
        [BindProperty]
        public CharacterModel Character { get; set; }

        [BindProperty]
        public IFormFile CharacterImage { get; set; }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public CharacterInput(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "You must be logged in.");
                return Page();
            }

            // Load the actual user so we can get User_ID (int)
            var userAccess = new UserAccessLayer(_configuration);
            var user = userAccess.GetUserByUsername(username);

            int userId = user.User_ID;

            // Assign character to correct user ID
            Character.Creator_ID = userId.ToString();

            CharacterAccessLayer factory = new CharacterAccessLayer(_configuration);

            // Check if user already has 4 characters
            if (factory.CountByCreatorId(userId.ToString()) >= 4)
            {
                ModelState.AddModelError("", "You already have 4 characters.");
                return Page();
            }

            // Handle file upload
            // Save character image to wwwroot/images/characters
            if (CharacterImage != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "images", "characters");
                Directory.CreateDirectory(folder);

                string file = Guid.NewGuid().ToString() + Path.GetExtension(CharacterImage.FileName);
                string path = Path.Combine(folder, file);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    CharacterImage.CopyTo(stream);
                }

                // Store relative path for SQL + Razor
                Character.Image_Path = $"/images/characters/{file}";
            }

            factory.create(Character);

            return RedirectToPage("/Profile", new { slot = Character.Slots });
        }
    }
}