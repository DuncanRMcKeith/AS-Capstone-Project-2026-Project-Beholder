using CapstoneProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CapstoneProject.Pages
{
    public class CharacterInput : PageModel
    {
        [BindProperty]
        public CharacterModel Character { get; set; }
        private readonly IConfiguration _configuration;

        public CharacterInput(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
        }


        public IActionResult OnPost()
        {
            CharacterAccessLayer factory = new CharacterAccessLayer(_configuration);
            factory.create(Character);
            return Page();
        }
    }

}