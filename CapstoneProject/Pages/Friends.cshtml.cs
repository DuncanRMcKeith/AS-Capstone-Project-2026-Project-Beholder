using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class FriendsModel : PageModel
    {
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public List<UserModel> Requests { get; set; } = new List<UserModel>();
        public List<UserModel> Search { get; set; } = new();

        private readonly UserAccessLayer _userAccess;

        public FriendsModel(UserAccessLayer userAccess)
        {
            _userAccess = userAccess;
        }

        public UserModel CurrentUser { get; set; }
        
        public IActionResult OnGet()
        {

            var loggedIn = HttpContext.Session.GetString("LoggedIn");
            var username = HttpContext.Session.GetString("Username");

            if (loggedIn != "true") { 
                return RedirectToPage("/Login");
                }

            CurrentUser = _userAccess.GetUserByUsername(username);

            if (CurrentUser == null)
                return RedirectToPage("/Login");


            Users = _userAccess.GetFriends(HttpContext.Session.GetInt32("UserID").Value).ToList();
            Requests = _userAccess.GetFriendRequests(HttpContext.Session.GetInt32("UserID").Value).ToList();
            return Page();
    }

        public IActionResult OnPostAccept(int reqID)
        {
            int? userid = HttpContext.Session.GetInt32("UserID");

            _userAccess.AddFriend(userid.Value, reqID);
            _userAccess.RemoveRequest(userid.Value, reqID);

            return RedirectToPage();
        }

        public IActionResult OnPostReject(int reqID)
        {
            int? userid = HttpContext.Session.GetInt32("UserID");

            _userAccess.RemoveRequest(userid.Value, reqID);

            return RedirectToPage();
        }

        //[IgnoreAntiforgeryToken] //used for debugging purposes, if the remove friend button is giving a 400 error, un-comment this
        public IActionResult OnPostRemoveFriend(int friendid)
        {
            int? userid = HttpContext.Session.GetInt32("UserID");
            _userAccess.RemoveFriend(userid.Value, friendid);

            return new JsonResult(new {success = true});
        }

        public IActionResult OnPostAddFriendRequest(int friendid)
        {
            int? userid = HttpContext.Session.GetInt32("UserID");
            _userAccess.AddFriendRequest(userid.Value, friendid);
            return Page();
        }

        public IActionResult OnGetSearch(string term)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserID");

            if (!string.IsNullOrWhiteSpace(term))
            {
                Search = _userAccess.SearchUsers(term, currentUserId.Value).ToList();


            }
            return Partial("Search", Search);
        }

}
}
