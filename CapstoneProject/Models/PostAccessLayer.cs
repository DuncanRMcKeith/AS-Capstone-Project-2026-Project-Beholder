using Azure.Core.Pipeline;
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
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

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
                    SELECT TOP 10 p.Post_ID, p.Title, p.Content, p.Created_At, p.Creator_ID, 
                    u.Username AS CreatorUsername, u.Profilepic AS CreatorProfilepic
                    FROM Posts p
                    JOIN Users u ON p.Creator_ID = u.User_ID
                    ORDER BY p.Created_At DESC
                    ";
                    SqlCommand Cmd = new SqlCommand(strsql, conn);
                    
                    Cmd.CommandType = CommandType.Text;

                    conn.Open();
                    SqlDataReader rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        posts.Add(new PostsModel
                        {
                            Post_ID = Convert.ToInt32(rdr["Post_ID"]),
                            Title = Convert.ToString(rdr["Title"]),
                            Content = Convert.ToString(rdr["Content"]),
                            Created_At = Convert.ToDateTime(rdr["Created_at"]),
                            Creator_ID = Convert.ToInt32(rdr["Creator_ID"]),
                            CreatorUsername = Convert.ToString(rdr["CreatorUsername"]),
                            CreatorProfilepic = rdr["CreatorProfilepic"] == DBNull.Value ? null : rdr["CreatorProfilepic"].ToString()
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

        public IEnumerable<PostsModel> getPosts(int page, int pageSize)
        {
            List<PostsModel> posts = new List<PostsModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string strsql = @"
                SELECT p.Post_ID, p.Title, p.Content, p.Created_At, p.Creator_ID,
                       u.Username AS CreatorUsername, u.Profilepic AS CreatorProfilepic
                FROM Posts p
                JOIN Users u ON p.Creator_ID = u.User_ID
                ORDER BY p.Created_At DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY
            ";
                    SqlCommand cmd = new SqlCommand(strsql, conn);
                    cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        posts.Add(new PostsModel
                        {
                            Post_ID = Convert.ToInt32(rdr["Post_ID"]),
                            Title = Convert.ToString(rdr["Title"]),
                            Content = Convert.ToString(rdr["Content"]),
                            Created_At = Convert.ToDateTime(rdr["Created_At"]),
                            Creator_ID = Convert.ToInt32(rdr["Creator_ID"]),
                            CreatorUsername = Convert.ToString(rdr["CreatorUsername"]),
                            CreatorProfilepic = rdr["CreatorProfilepic"] == DBNull.Value ? null : rdr["CreatorProfilepic"].ToString()
                        });
                    }
                }
            }
            catch (Exception err) { }
            return posts;
        }

        public int GetPostCount()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Posts", conn);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public IEnumerable<CommentModel> GetComments(int postId)
        {
            List<CommentModel> comments = new List<CommentModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT c.Comment_ID, c.Post_ID, c.User_ID, u.Username, u.Profilepic, c.Content, c.Created_At FROM Comments c JOIN Users u ON c.User_ID = u.User_ID WHERE c.Post_ID = @postId ORDER BY c.Created_At ASC";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    try
                    {
                        command.Parameters.AddWithValue("@postId", postId);
                        connection.Open();
                        SqlDataReader rdr = command.ExecuteReader();
                        while (rdr.Read())
                        {
                            comments.Add(new CommentModel
                            {
                                Comment_ID = Convert.ToInt32(rdr["Comment_ID"]),
                                Post_ID = Convert.ToInt32(rdr["Post_ID"]),
                                User_ID = Convert.ToInt32(rdr["User_ID"]),
                                Username = Convert.ToString(rdr["Username"]),
                                Content = Convert.ToString(rdr["Content"]),
                                Profilepic = rdr["Profilepic"] == DBNull.Value ? null : rdr["Profilepic"].ToString()
                            });
                        }
                    }
                    catch(Exception err)
                    {

                    }
                }
            }
            return comments;
        }

        public void AddComment(int postId, int userId, string content)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Comments (Post_ID, User_ID, Content) VALUES (@postId, @userId, @content)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@postId", postId);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@content", content);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void DeleteComment(int commentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM Comments WHERE Comment_ID = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", commentId);


                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

            }
        }
        public void DeletePost(int postId)
        {
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM Posts WHERE Post_ID = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", postId);
                    

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

            }
            
        }

        public void ToggleLike(int postId, int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
            IF EXISTS (SELECT 1 FROM Likes WHERE Post_ID = @postId AND User_ID = @userId)
                DELETE FROM Likes WHERE Post_ID = @postId AND User_ID = @userId
            ELSE
                INSERT INTO Likes (Post_ID, User_ID) VALUES (@postId, @userId);
        ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@postId", postId);
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public int GetLikeCount(int postId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Likes WHERE Post_ID = @postId", conn);
                cmd.Parameters.AddWithValue("@postId", postId);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool IsLikedByUser(int postId, int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Likes WHERE Post_ID = @postId AND User_ID = @userId", conn);
                cmd.Parameters.AddWithValue("@postId", postId);
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }

}
