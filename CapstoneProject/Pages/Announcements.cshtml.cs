using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class AnnouncementsModel : PageModel
    {

        private readonly IConfiguration _configuration;

        public AnnouncementsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<PostsModel> Posts { get; set; } = new();
        public List<UserModel> User_ID { get; set; } = new();

        public void OnGet()
        {
            var factory = new AnnouncementsAccessLayer(_configuration);
            Posts = factory.GetTop10Posts();
            User_ID = factory.GetUsersID();
        }
    }
}
