using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using CapstoneProject.Models;

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

            IActionResult temp;

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = "SELECT Password FROM Users WHERE Username = @username";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", uname);

            var result = cmd.ExecuteScalar();

            if (result == null)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                temp = Page();
            }

            string storedPassword = result.ToString();

            var hasher = new PasswordHasher<UserModel>();

            var verificationResult = hasher.VerifyHashedPassword(
                null,
                storedPassword,
                psw
                );

            if (verificationResult != PasswordVerificationResult.Success)
            {
                TempData["Login"] = "fail";
                ModelState.AddModelError("", "Username or password is incorrect");
                temp = Page();
                
            }

            else
            {
                HttpContext.Session.SetString("LoggedIn", "true");
                HttpContext.Session.SetString("Username", uname);
                temp = RedirectToPage("/Profile");
            }
                

            return temp;
        }
    }
}