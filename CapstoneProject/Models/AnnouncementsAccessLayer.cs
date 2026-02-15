using CapstoneProject.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CapstoneProject.Models
{
    public class AnnouncementsAccessLayer
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public AnnouncementsAccessLayer(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        // Get top 10 newest posts
        public List<PostsModel> GetTop10Posts()
        {
            var posts = new List<PostsModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                string sql = @"
                SELECT TOP (10)
                    Post_ID, Title, Post_text, Creator_ID, Comm_ID, Created_At
                FROM Posts
                ORDER BY Created_At DESC;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            posts.Add(new PostsModel
                            {
                                Post_ID = reader.GetInt32(reader.GetOrdinal("Post_ID")),
                                Title = reader["Title"] as string ?? "",
                                Post_text = reader["Post_text"] as string ?? "",
                                Creator_ID = Convert.ToInt32(reader["Creator_ID"]),  
                                Comm_ID = reader["Comm_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Comm_ID"]),
                                Created_At = Convert.ToDateTime(reader["Created_At"])
                            });
                        }
                    }
                }
            }

            return posts;
        }
    }
}
