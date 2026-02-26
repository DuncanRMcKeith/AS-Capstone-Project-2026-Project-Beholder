using Microsoft.Data.SqlClient;
using System.Data;

namespace CapstoneProject.Models
{
    public class CommunityDataAccessLayer
    {
        string? connectionString;
        private readonly IConfiguration _configuration;

        public CommunityDataAccessLayer(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<CommunityModel> GetAll()
        {
            List<CommunityModel> communities = new List<CommunityModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "SELECT * FROM Community";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CommunityModel community = new CommunityModel();
                        community.CommunityID = Convert.ToInt32(reader["Comm_ID"]);
                        community.Name = reader["Name"].ToString();
                        community.CampaignVersion = reader["CampaignVersion"].ToString();
                        community.Description = reader["Description"].ToString();
                        community.Badges = reader["Badges"].ToString();
                        community.MemberCount = Convert.ToInt32(reader["User_Count"]);
                        community.ImageURL = reader["ImageURL"].ToString();
                        communities.Add(community);
                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("GetAll error: " + err.Message);
            }
            return communities;
        }

        public IEnumerable<CommunityModel> GetUserCommunities(int userID)
        {
            List<CommunityModel> communities = new List<CommunityModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT c.Comm_ID, c.Name, c.Description, c.CampaignVersion, c.Badges, c.ImageURL, c.User_Count
                                   FROM Community c
                                   INNER JOIN Members m ON c.Comm_ID = m.Comm_ID
                                   WHERE m.User_ID = @userID";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        communities.Add(new CommunityModel
                        {
                            CommunityID = Convert.ToInt32(reader["Comm_ID"]),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            CampaignVersion = reader["CampaignVersion"].ToString(),
                            Badges = reader["Badges"].ToString(),
                            MemberCount = Convert.ToInt32(reader["User_Count"]),
                            ImageURL = reader["ImageURL"].ToString()
                        });
                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("GetUserCommunities error: " + err.Message);
            }
            return communities;
        }

        // Creates a new community and returns the new Comm_ID
        public int CreateCommunity(CommunityModel community, int creatorID)
        {
            int newId = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"INSERT INTO Community (Name, Description, CampaignVersion, Badges, ImageURL, Creator_ID, User_Count, Created_date)
                                   OUTPUT INSERTED.Comm_ID
                                   VALUES (@name, @description, @campaignVersion, @badges, @imageURL, @creatorId, 1, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", community.Name);
                        cmd.Parameters.AddWithValue("@description", community.Description);
                        cmd.Parameters.AddWithValue("@campaignVersion", community.CampaignVersion);
                        cmd.Parameters.AddWithValue("@badges", community.Badges);
                        cmd.Parameters.AddWithValue("@imageURL", community.ImageURL);
                        cmd.Parameters.AddWithValue("@creatorId", creatorID);

                        conn.Open();
                        newId = (int)cmd.ExecuteScalar();
                        conn.Close();
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("CreateCommunity error: " + err.Message);
            }
            return newId;
        }

        // Joins a community as admin and leader
        public void JoinCommunityAsAdmin(int userID, int communityID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"INSERT INTO Members (Comm_ID, User_ID, Is_admin, Role, Join_date)
                                   VALUES (@commId, @userId, 1, 'Leader', GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@commId", communityID);
                        cmd.Parameters.AddWithValue("@userId", userID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("JoinCommunityAsAdmin error: " + err.Message);
            }
        }

        public void JoinCommunity(int userID, int communityID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"INSERT INTO Members (Comm_ID, User_ID, Is_admin, Role, Join_date)
                                   VALUES (@commId, @userId, 0, 'Member', GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@commId", communityID);
                        cmd.Parameters.AddWithValue("@userId", userID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    string updateSql = "UPDATE Community SET User_Count = User_Count + 1 WHERE Comm_ID = @commId";
                    using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@commId", communityID);
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("JoinCommunity error: " + err.Message);
            }
        }

        public void LeaveCommunity(int userID, int communityID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "DELETE FROM Members WHERE User_ID = @userId AND Comm_ID = @commId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userID);
                        cmd.Parameters.AddWithValue("@commId", communityID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    string updateSql = "UPDATE Community SET User_Count = User_Count - 1 WHERE Comm_ID = @commId";
                    using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@commId", communityID);
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("LeaveCommunity error: " + err.Message);
            }
        }
    }
}