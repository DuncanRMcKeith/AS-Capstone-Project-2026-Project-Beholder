using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace CapstoneProject.Models
{
    public class CommunityDataAccessLayer
    {
        string? connectionString;

        private readonly IConfiguration _configuration;

        public CommunityDataAccessLayer(IConfiguration configuration)
        {
            _configuration = configuration;
            //Need to add your server connectivity to the JSON file
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<CommunityModel> GetAll()
        {
            List<CommunityModel> communities = new List<CommunityModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM TABLENAME"; //Preferrably the name of community table
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CommunityModel community = new CommunityModel();

                        //Grabs each attribute for the community then saves it to a list of communities
                        community.Name = reader["Name"].ToString();
                        community.CampaignVersion = reader["CampaignVersion"].ToString();
                        community.Description = reader["Description"].ToString();
                        community.Badges = reader["Badges"].ToString();
                        community.MemberCount = Convert.ToInt32(reader["MemberCount"]);
                        community.ImageURL = reader["ImageURL"].ToString();

                        communities.Add(community);
                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            {
                //Nothing
            }
            return communities;
        }
    }
}
