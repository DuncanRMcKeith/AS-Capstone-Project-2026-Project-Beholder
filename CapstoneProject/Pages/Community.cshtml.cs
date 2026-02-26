using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class CommunityModel : PageModel
    {
        private readonly IConfiguration _configuration;
        CommunityDataAccessLayer factory;

        public List<CapstoneProject.Models.CommunityModel> Communities { get; set; }
        public List<int> JoinedCommunityIds { get; set; } = new List<int>();

        public CommunityModel(IConfiguration configuration)
        {
            _configuration = configuration;
            factory = new CommunityDataAccessLayer(configuration);
        }

        public void OnGet()
        {
            Communities = factory.GetAll().ToList();

            // Get the communities the user has already joined
            var userIdFromSession = HttpContext.Session.GetInt32("UserID");
            if (userIdFromSession.HasValue)
            {
                JoinedCommunityIds = factory.GetUserCommunities(userIdFromSession.Value)
                    .Select(c => c.CommunityID)
                    .ToList();
            }
        }

        public IActionResult OnPostJoin(int communityId)
        {
            var userIdFromSession = HttpContext.Session.GetInt32("UserID");
            if (!userIdFromSession.HasValue)
            {
                return RedirectToPage("/Login");
            }

            factory.JoinCommunity(userIdFromSession.Value, communityId);
            return RedirectToPage();
        }

        public IActionResult OnPostLeave(int communityId)
        {
            var userIdFromSession = HttpContext.Session.GetInt32("UserID");
            if (!userIdFromSession.HasValue)
            {
                return RedirectToPage("/Login");
            }

            factory.LeaveCommunity(userIdFromSession.Value, communityId);
            return RedirectToPage();
        }
    }
}
