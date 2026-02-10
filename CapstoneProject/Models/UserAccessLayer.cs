using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using CapstoneProject.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;

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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Users (Username, Email, Password, ProfilePicture, Created_Date) VALUES (@user, @email, @password, 'default.png', GETDATE())";


                try
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@user", user.Username);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@password", user.Password);

                        connection.Open();
                        user.Feedback = command.ExecuteNonQuery().ToString() + "Success!";
                        connection.Close();

                    }
                }
                catch (Exception err)
                {
                    user.Feedback = "ERROR: " + err.Message;
                }
            }

        }
    }
}
