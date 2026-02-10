using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace CapstoneProject.Pages
{
    public class CharacterModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public CharacterModel(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        [BindProperty]
        public CharacterInput Input { get; set; } = new();

        
        [BindProperty]
        public IFormFile? CharacterImage { get; set; }
        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            // session login
            int? creatorId = HttpContext.Session.GetInt32("UserId");
            if (creatorId == null)
                return RedirectToPage("/Login");

            string imagePath = "";

            // Save img 
            if (CharacterImage != null && CharacterImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(CharacterImage.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await CharacterImage.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName;
            }

            // SQL connection
            string connString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();

                string sql = @"
            INSERT INTO Characters
            (Creator_ID, FName, LName, Title, Level, Char_class,
             Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma, Notes, Image_Path)
            VALUES
            (@Creator_ID, @FName, @LName, @Title, @Level, @Char_class,
             @Strength, @Dexterity, @Constitution, @Intelligence, @Wisdom, @Charisma, @Notes, @Image_Path)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Creator_ID", creatorId.Value);
                    cmd.Parameters.AddWithValue("@FName", Input.FName ?? "");
                    cmd.Parameters.AddWithValue("@LName", Input.LName ?? "");
                    cmd.Parameters.AddWithValue("@Title", Input.Title ?? "");
                    cmd.Parameters.AddWithValue("@Level", Input.Level);
                    cmd.Parameters.AddWithValue("@Char_class", Input.CharacterClass ?? "");

                    cmd.Parameters.AddWithValue("@Strength", Input.Strength);
                    cmd.Parameters.AddWithValue("@Dexterity", Input.Dexterity);
                    cmd.Parameters.AddWithValue("@Constitution", Input.Constitution);
                    cmd.Parameters.AddWithValue("@Intelligence", Input.Intelligence);
                    cmd.Parameters.AddWithValue("@Wisdom", Input.Wisdom);
                    cmd.Parameters.AddWithValue("@Charisma", Input.Charisma);

                    cmd.Parameters.AddWithValue("@Notes", Input.Notes ?? "");
                    cmd.Parameters.AddWithValue("@Image_Path", imagePath);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return RedirectToPage();
        }
        public class CharacterInput
        {
            public string FName { get; set; } = "";
            public string LName { get; set; } = "";
            public string Title { get; set; } = "";
            public int Level { get; set; } = 0;
            public string CharacterClass { get; set; } = "";
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