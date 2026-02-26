using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class CreateCommunityModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        CommunityDataAccessLayer factory;

        public string ErrorMessage { get; set; }

        public CreateCommunityModel(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            factory = new CommunityDataAccessLayer(configuration);
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(
            string name,
            string description,
            string campaignVersion,
            string badges,
            IFormFile imageFile)
        {
            var userIdFromSession = HttpContext.Session.GetInt32("UserID");
            if (!userIdFromSession.HasValue)
            {
                return RedirectToPage("/Login");
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            {
                ErrorMessage = "Name and description are required.";
                return Page();
            }

            // Handle image upload
            string imageURL = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate a new filename
                string extension = Path.GetExtension(imageFile.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;
                string folderPath = Path.Combine(_environment.WebRootPath, "Images", "Community");

                // Create folder if it doesn't exist
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                imageURL = "/Images/Community/" + fileName;
            }

            // Create the community
            int newCommunityId = factory.CreateCommunity(new CapstoneProject.Models.CommunityModel
            {
                Name = name,
                Description = description,
                CampaignVersion = campaignVersion ?? "",
                Badges = badges ?? "",
                ImageURL = imageURL ?? "",
                MemberCount = 1
            }, userIdFromSession.Value);

            // Auto join as admin and leader
            factory.JoinCommunityAsAdmin(userIdFromSession.Value, newCommunityId);

            return RedirectToPage("/Community");
        }
    }
}
