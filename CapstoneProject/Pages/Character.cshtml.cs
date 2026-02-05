using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace CapstoneProject.Pages
{
    public class CharacterModel : PageModel
    {
        [BindProperty]
        public CharacterInput Input { get; set; } = new();

        public void OnGet() { }
        public void OnPost() { }

        public class CharacterInput
        {
            public string Name { get; set; } = "";
            public int Strength { get; set; } = 0;
            public int Dexterity { get; set; } = 0;
            public int Constitution { get; set; } = 0;
            public int Intelligence { get; set; } = 0;
            public int Wisdom { get; set; } = 0;
            public int Charisma { get; set; } = 0;
            public string Notes { get; set; } = "";
        }
    }
}
