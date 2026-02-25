using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class CommunityModel : PageModel
    {
        private readonly IConfiguration _configuration;

        CommunityDataAccessLayer factory;

        // Change the type to CapstoneProject.Models.CommunityModel
        public List<CapstoneProject.Models.CommunityModel> Communities { get; set; }

        public CommunityModel(IConfiguration configuration)
        {
            _configuration = configuration;

            factory = new CommunityDataAccessLayer(configuration);
        }

        public void OnGet()
        {
            Communities = factory.GetAll().ToList();
        }
    }
}