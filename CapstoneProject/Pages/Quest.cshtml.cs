using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace CapstoneProject.Pages
{
    public class QuestModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        private const int PageSize = 10;


        public int? userId { get; set; }
        public string? LoggedIn { get; set; }
        private readonly PostAccessLayer _PostAccessLayer;
        public List<PostsModel> Posts { get; set; } = new List<PostsModel>();

        public QuestModel(PostAccessLayer postAccessLayer)
        {
            _PostAccessLayer = postAccessLayer;
        }
        public IActionResult OnGet()
        {
            userId = HttpContext.Session.GetInt32("UserID");
            LoggedIn = HttpContext.Session.GetString("LoggedIn");
            var username = HttpContext.Session.GetString("Username");

            int totalPosts = _PostAccessLayer.GetPostCount();
            TotalPages = (int)Math.Ceiling(totalPosts / (double)PageSize);
            Posts = _PostAccessLayer.getPosts(CurrentPage, PageSize).ToList();
                
            
            foreach (var post in Posts)
            {
                post.Comments = _PostAccessLayer.GetComments(post.Post_ID).ToList();
                post.Likes = _PostAccessLayer.GetLikeCount(post.Post_ID);
                post.IsLikedByCurrentUser = userId.HasValue && _PostAccessLayer.IsLikedByUser(post.Post_ID, userId.Value);
            }
                return Page();
           

        }

        [BindProperty]
        public PostsModel Post { get; set; }

        public IActionResult OnPostAddPost()
        {
            int? Id = HttpContext.Session.GetInt32("UserID");

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
                    Post.Creator_ID = Id.Value;
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

        
        public int CommentPostId { get; set; }
        
        public string CommentContent { get; set; }

        public IActionResult OnPostAddComment()
        {
            userId = HttpContext.Session.GetInt32("UserID");

            int postId = int.Parse(Request.Form["CommentPostId"]);
            string content = Request.Form["CommentContent"];

            if (string.IsNullOrWhiteSpace(content) || userId == null)
            {
                return RedirectToPage("/Quest");
            }

            _PostAccessLayer.AddComment(postId, userId.Value, content);
            return RedirectToPage("/Quest");
        }

        public int postid { get; set; }
        public IActionResult OnPostRemovePost()
        {
            int postId = int.Parse(Request.Form["postid"]);
            _PostAccessLayer.DeletePost(postId);
            return RedirectToPage("/Quest");

        }
        public int commentId { get; set; }
        public IActionResult OnPostRemoveComment()
        {
            int commentId = int.Parse(Request.Form["commentid"]);
            _PostAccessLayer.DeleteComment(commentId);
            return RedirectToPage("/Quest");

        }

        public IActionResult OnPostToggleLike()
        {
            userId = HttpContext.Session.GetInt32("UserID");
            int postId = int.Parse(Request.Form["postId"]);

            if (userId == null) return new JsonResult(new { success = false });

            _PostAccessLayer.ToggleLike(postId, userId.Value);
            int newCount = _PostAccessLayer.GetLikeCount(postId);
            bool isLiked = _PostAccessLayer.IsLikedByUser(postId, userId.Value);

            return new JsonResult(new { success = true, likes = newCount, isLiked });
        }
    }
}
