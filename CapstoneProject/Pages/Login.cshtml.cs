using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

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
        public string uname { get; set; }

        [BindProperty]
        public string psw { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = "SELECT Password FROM Users WHERE Username = @username";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", uname);

            var result = cmd.ExecuteScalar();

            if (result == null)
            {
                ErrorMessage = "User not found";
                return Page();
            }

            string storedPassword = result.ToString();

            if (storedPassword != psw)
            {
                ErrorMessage = "Incorrect password";
                return Page();
            }

            HttpContext.Session.SetString("LoggedIn", "true");
            HttpContext.Session.SetString("Username", uname);

            return RedirectToPage("/Index");
        }
    }
}