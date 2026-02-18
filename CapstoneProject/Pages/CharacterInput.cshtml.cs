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

        public IActionResult OnGet()
        {
            var loggedIn = HttpContext.Session.GetString("LoggedIn");
            if (loggedIn != "true")
            {
                return RedirectToPage("/Login");
            }
            else
            {
                return Page();
            }
        }

        public IActionResult OnPost()

        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            //if (string.IsNullOrEmpty(userId))
            //{
            //    // Not logged in 
            //    ModelState.AddModelError("", "You must be logged in.");
            //    return Page();
            //}

            // Assign character to userId
            Character.Creator_ID = userId.Value;
            // Character.Creator_ID = "38"; test line to set character to userId that is registered

            CharacterAccessLayer factory = new CharacterAccessLayer(_configuration);

            // Check if user already has 4 characters
            if (factory.CountByCreatorId(userId.Value) >= 4)
            {
                ModelState.AddModelError("", "You already have 4 characters.");
                return Page();
            }

            /*if (factory.CountByCreatorId(Character.Creator_ID) >= 4)
            {
                ModelState.AddModelError("", "You already have 4 characters.");
                return Page();
            }*/ // test line used to see if connection to database was succesful, should be removed when userId is implemented

            // Handle file upload allowed null
            if (CharacterImage != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(folder);

                string file = Guid.NewGuid().ToString() + Path.GetExtension(CharacterImage.FileName);
                string path = Path.Combine(folder, file);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    CharacterImage.CopyTo(stream);
                }

                Character.Image_Path = "/uploads/" + file;
            }
            factory.create(Character);
            return Page();
        }
    }
}