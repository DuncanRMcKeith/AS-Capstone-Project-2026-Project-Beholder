using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class FriendsModel : PageModel
    {
        public List<string> friends { get; set; }
        public List<string> requests { get; set; }

        public void OnGet()
        {
            friends = new List<string>
            {
                "Andrew",
                "Kyle",
                "Rob",
                "Duncan"
            };
            requests = new List<string>
            {
                "user1",
                "user2",
                "user3",
                "user1",
                "user2",
                "user3",
                "user1",
                "user2",
                "user3",
                "user1",
                "user2",
                "user3",
                "user1",
                "user2",
                "user3",
                "user1",
                "user2",
                "user3",
                "user1",
                "user2",
                "user3"
            };
        }
    }
}
