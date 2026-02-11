namespace CapstoneProject.Models
{
    public class CommunityModel
    {
        //Need the 
        //Community, Versions of DND played, Description, # of members, badges, image

        public int CommunityID { get; set; }

        public string Name { get; set; }

        public string CampaignVersion { get; set; }

        public string Description { get; set; }

        public string Badges { get; set; }

        public int MemberCount { get; set; }

        public string ImageURL { get; set; }
    }
}
