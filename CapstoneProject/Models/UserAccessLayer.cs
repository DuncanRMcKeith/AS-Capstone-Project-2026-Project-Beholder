using CapstoneProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneProject.Models
{
    public class UserAccessLayer
    {
        string connectionString;

        private readonly IConfiguration _configuration;

        public UserAccessLayer(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public void create(UserModel user)
        {

            //PASSWORD HASHING
            var hasher = new PasswordHasher<UserModel>();

            //hash password
            user.Password = hasher.HashPassword(user, user.Password);


            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Users (Username, Email, Password, ProfilePicture, Created_Date) VALUES (@user, @email, @password, 'default.png', GETDATE())";


                
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@user", user.Username);
                        command.Parameters.AddWithValue("@email", user.Email.Trim().ToLower());
                        command.Parameters.AddWithValue("@password", user.Password);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                    }
            }

        }


        public UserModel GetUserByUsername(string username)
        {
            UserModel user = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Users WHERE Username = @username";
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@username", username);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new UserModel();
                                {
                                    user.User_ID = Convert.ToInt32(reader["User_ID"]);
                                    user.Username = reader["Username"].ToString();
                                    user.Email = reader["Email"].ToString();
                                    user.User_Description = reader["User_Description"].ToString();
                                };
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("ERROR: " + err.Message);
                }
            }
            return user;
        }


        public int? GetUserID(string username)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = "SELECT User_ID FROM Users WHERE Username = @username";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);

            var result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value)
            {
                return null;
            }
            return Convert.ToInt32(result);
        }

        public IEnumerable<UserModel> GetFriends(int userID)
        {
            List<UserModel> lstusers = new List<UserModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string strsql = @"
                    SELECT u.User_ID, u.Username, u.ProfilePicture
                    FROM Users u
                    INNER JOIN Friends f ON (f.User1 = @UserId AND u.User_ID = f.User2)
                    OR (f.User2 = @UserId AND u.User_ID = f.User1)
                    ";
                    SqlCommand Cmd = new SqlCommand(strsql, conn);
                    Cmd.Parameters.AddWithValue("@UserId", userID);
                    Cmd.CommandType = CommandType.Text;

                    conn.Open();
                    SqlDataReader rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        lstusers.Add(new UserModel
                        {
                            User_ID = Convert.ToInt32(rdr["User_ID"]),
                            Username = Convert.ToString(rdr["Username"]),
                            Profilepic = Convert.ToString(rdr["ProfilePicture"])
                        });
                        
                    }
                    conn.Close();
                }
            }
            catch(Exception err)
            {
            
            }
            return lstusers;
        }
    }
}
