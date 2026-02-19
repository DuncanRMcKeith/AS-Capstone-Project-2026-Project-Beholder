using CapstoneProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.Pipeline;

namespace CapstoneProject.Models
{
    public class PostAccessLayer
    {
        string connectionString;

        private readonly IConfiguration _configuration;

        public PostAccessLayer(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }


        public void CreatePost(PostsModel post)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Posts (Title, Content, Creator_ID, Comm_ID, Created_At) VALUES (@title, @content, @userid, @commid, GETDATE())";



                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@title", post.Title);
                    command.Parameters.AddWithValue("@content", post.Content);
                    command.Parameters.AddWithValue("@userid", post.Creator_ID);
                    if (post.Comm_ID == null)
                    {
                        command.Parameters.AddWithValue("@commid", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@commid", post.Comm_ID);
                    }
                        


                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }
            }
        }

        public IEnumerable<PostsModel> getPosts()
        {
            List<PostsModel> posts = new List<PostsModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string strsql = @"
                    Select top 10 * from Posts
                    ";
                    SqlCommand Cmd = new SqlCommand(strsql, conn);
                    
                    Cmd.CommandType = CommandType.Text;

                    conn.Open();
                    SqlDataReader rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        posts.Add(new PostsModel
                        {
                            Title = Convert.ToString(rdr["Title"]),
                            Content = Convert.ToString(rdr["Content"]),
                            Created_At = Convert.ToDateTime(rdr["Created_at"])
                        });

                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            {

            }
            return posts;
        }

    }

}
