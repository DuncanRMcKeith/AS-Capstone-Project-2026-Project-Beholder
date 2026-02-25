using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CapstoneProject.Pages
{
    public class CommunityModel : PageModel
    {


        private readonly IConfiguration _configuration;


        CommunityDataAccessLayer factory;

        public List<CommunityModel> Communities { get; set; }

        public CommunityModel(IConfiguration configuration)
        {
            _configuration = configuration;

            factory = new CommunityDataAccessLayer(configuration);
        }

        public void OnGet()
        {
            //Need to figure out this shi
            ////It's fucking broken
            //Communities = factory.GetAll().ToList();
        }
    }
}