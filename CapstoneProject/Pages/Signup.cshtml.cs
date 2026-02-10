using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class SignupModel : PageModel
    {
        [BindProperty]
        public UserModel? user { get; set; }

        public readonly IConfiguration _configuration;

        public SignupModel(IConfiguration configuration)
        {
            _configuration = configuration;

        }


        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            IActionResult temp;

           

            if (!ModelState.IsValid)
            {
                //no bueno
                temp = Page();
            }
            else
            {
                //bueno
                if(user != null)
                {
                    UserAccessLayer factory = new UserAccessLayer(_configuration);
                    factory.create(user);
                }
                temp = Page();
            }
            return temp;

        }
    }
}
