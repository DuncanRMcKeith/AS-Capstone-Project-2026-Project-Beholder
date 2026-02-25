using CapstoneProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CapstoneProject.Pages
{
    public class CharacterInput : PageModel
    {
        [BindProperty]
        public CharacterModel Character { get; set; }

        [BindProperty]
        [ValidateNever]
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
            var userId = HttpContext.Session.GetInt32("UserID").ToString();

            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "You must be logged in.");
                return Page();
            }

            // Remove error if image is not uploaded 
            ModelState.Remove("CharacterImage");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Assign character to userId
            Character.Creator_ID = int.Parse(userId);

            // Character.Creator_ID = "38"; test line to set character to userId that is registered

            CharacterAccessLayer factory = new CharacterAccessLayer(_configuration);

            // Check if user already has 4 characters
            if (factory.CountByCreatorId(int.Parse(userId)) >= 4)
            {
                ModelState.AddModelError("", "You already have 4 characters.");
                return Page();
            }

            // Handle file upload allowed null
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