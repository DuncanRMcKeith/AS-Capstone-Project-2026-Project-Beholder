using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace CapstoneProject.Pages
{
    public class QuestModel : PageModel
    {

        private readonly PostAccessLayer _PostAccessLayer;
        public List<PostsModel> Posts { get; set; } = new List<PostsModel>();

        public QuestModel(PostAccessLayer postAccessLayer)
        {
            _PostAccessLayer = postAccessLayer;
        }
        public IActionResult OnGet()
        {

            var loggedIn = HttpContext.Session.GetString("LoggedIn");
            var username = HttpContext.Session.GetString("Username");

            //if (loggedIn != "true")
            //{
            //    return RedirectToPage("/Login");
            //}
            //else
            //{
            Posts = _PostAccessLayer.getPosts().ToList();
                return Page();
            //}


           

        }

        [BindProperty]
        public PostsModel Post { get; set; }

        public IActionResult OnPost()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            IActionResult temp;



            if (!ModelState.IsValid)
            {
                //no bueno
                ModelState.AddModelError("", "Something went wrong");
                temp = Page();
            }
            else
            {
                try
                {
                    Post.Creator_ID = userId.Value;
                    _PostAccessLayer.CreatePost(Post);
                    temp = RedirectToPage("/Quest"); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    temp = Page();
                }
            }
            return temp;

        }
    }
}
