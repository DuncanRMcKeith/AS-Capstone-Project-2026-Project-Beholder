using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace CapstoneProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly string _connectionString;

        public LoginModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            string sql = "SELECT password FROM users WHERE username = @username";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", Username);

            var result = cmd.ExecuteScalar();

            if (result == null)
            {
                ErrorMessage = "User not found";
                return Page();
            }

            string storedPassword = result.ToString();

            if (storedPassword != Password)
            {
                ErrorMessage = "Incorrect password";
                return Page();
            }

            HttpContext.Session.SetString("LoggedIn", "true");
            HttpContext.Session.SetString("Username", Username);

            return RedirectToPage("/Index");
        }
    }
}