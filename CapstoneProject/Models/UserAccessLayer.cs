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
                string sql = "INSERT INTO Users (Username, Email, Password, ProfilePicture, Created_Date, User_Description) VALUES (@user, @email, @password, 'default.png', GETDATE()), 'Hello!'";


                
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
    }
}
