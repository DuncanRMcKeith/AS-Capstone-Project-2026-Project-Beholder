using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using CapstoneProject.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Azure.Core.Pipeline;

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


            /*
             Stuff to decrypt the password

            var hasher = new PasswordHasher<UserModel>();

            var result = hasher.VerifyHashedPassword(
                userFromDb,                 // the DB user
                userFromDb.Password,        // hashed password from DB
                loginInputPassword          // plain text password from login form
            );

            if (result == PasswordVerificationResult.Success)
            {
                // Password correct
            }
            else
            {
                // Invalid login
            }
             
             
             */

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

        public UserModel UpdateBio(UserModel user)
        {
            UserModel updatedUser = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Users SET User_Description = @description WHERE Username = @username";
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@description", user.User_Description);
                        command.Parameters.AddWithValue("@username", user.Username);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        // Return the updated user
                        updatedUser = GetUserByUsername(user.Username);
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("ERROR: " + err.Message);
                }
            }
            return updatedUser;
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
                                    user.Password = reader["Password"].ToString();
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
    }
}
