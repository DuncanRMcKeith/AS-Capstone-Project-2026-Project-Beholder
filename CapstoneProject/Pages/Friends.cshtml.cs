using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class FriendsModel : PageModel
    {
        private readonly UserAccessLayer _userAccess;

        public FriendsModel(UserAccessLayer userAccess)
        {
            _userAccess = userAccess;
        }

        public UserModel CurrentUser { get; set; }
        UserAccessLayer factory;
        public List<UserModel> Users { get; set; }
        public IActionResult OnGet()
        {

            var loggedIn = HttpContext.Session.GetString("LoggedIn");
            var username = HttpContext.Session.GetString("Username");

            if (loggedIn != "true")
                return RedirectToPage("/Login");

            CurrentUser = _userAccess.GetUserByUsername(username);

            if (CurrentUser == null)
                return RedirectToPage("/Login");


            Users = factory.GetFriends().ToList();

            return Page();



            
            
    }
}
}
