using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace CapstoneProject.Pages
{

    

    public class SignupModel : PageModel
    {
        private readonly UserAccessLayer _userAccessLayer;

        public SignupModel(UserAccessLayer userAccessLayer)
        {
            _userAccessLayer = userAccessLayer;
        }


        [BindProperty]
        public UserModel? user { get; set; } = new UserModel();




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
                try
                {
                    _userAccessLayer.create(user);
                    temp = Page();
                    //temp = RedirectToPage("Login");
                    TempData["Signedup"] = "User has signed up";
                }     
                catch (SqlException ex)
                {
                    if(ex.Number == 2627 || ex.Number == 2601) //2627 and 2601 are errors generated when the UNIQUE quality in an SQL table isn't met
                    {
                        string msg = ex.Message;

                        //check to see if its the username or the email thats throwing a tantrum

                        if (msg.Contains("UQ_Users_Email"))
                        {
                            ModelState.AddModelError("", "Email is already taken.");
                            temp = Page();
                        }

                        else if (msg.Contains("UQ__Users__Username"))
                        {
                            ModelState.AddModelError("", "Username is already taken.");
                            temp = Page();
                        }

                        else
                        {
                            temp = Page();
                            ModelState.AddModelError("", "An unexpected error has occured, please try again");
                        }
                    }
                    else
                    {
                        throw; // smth else went wrong
                    }
                }
            }
            return temp;

        }
    }
}
