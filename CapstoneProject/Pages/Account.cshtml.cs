using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;

namespace CapstoneProject.Pages
{
    public class AccountModel : PageModel
    {
        private readonly UserAccessLayer _userAccess;
        public AccountModel(UserAccessLayer userAccess)
        {
            _userAccess = userAccess;
        }
        public UserModel User { get; set; }

        public IActionResult OnGet(int id)
        {
            IActionResult temp = Page();
            User = _userAccess.GetUserByID(id);   

            if (User == null)
            {
                temp = RedirectToPage("/Index");
            }
            return temp; 
        }
    }
}
