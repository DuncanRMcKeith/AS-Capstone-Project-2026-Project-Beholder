using CapstoneProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
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

        public IEnumerable<UserModel> GetFriends()
        {
            List<UserModel> lstusers = new List<UserModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string strsql = "SELECT * FROM Friends WHERE User1 = @user OR User2 = @user";
                    SqlCommand Cmd = new SqlCommand(strsql, conn);
                    Cmd.Parameters.AddWithValue("@user", );
                    Cmd.CommandType = CommandType.Text;

                    conn.Open();
                    SqlDataReader rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        UserModel user = new UserModel();
                        user.User_ID = Convert.ToInt32(rdr["User_ID"]);
                        user.Username = Convert.ToString(rdr["Username"]);
                        user.Profilepic = Convert.ToString(rdr["ProfilePicture"]);
                    }

                }
            }
            catch(Exception err)
            {

            }
            return lstusers;
        }


        //public IEnumerable<UserModel> GetActiveRecords()
        //{
        //    List<UserModel> lstusers = new List<UserModel>();

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(connectionString))
        //        {
        //            string strsql = "SELECT * FROM Friends ;";
        //            SqlCommand cmd = new SqlCommand(strsql, con);
        //            cmd.CommandType = CommandType.Text;

        //            con.Open();
        //            SqlDataReader rdr = cmd.ExecuteReader();

        //            while (rdr.Read())
        //            {
        //                UserModel game = new UserModel();
        //                game.Game_ID = Convert.ToInt32(rdr["Id"]);
        //                game.Title = rdr["Title"].ToString();
        //                game.Developer = rdr["Developer"].ToString();
        //                game.Genre = rdr["Genre"].ToString();
        //                game.Price = Convert.ToInt32(rdr["Price"]);
        //                game.Hours = Convert.ToInt32(rdr["Hours"]);
        //                game.Release_Date = DateTime.Parse(rdr["Release_Date"].ToString());

        //                lstTix.Add(game);
        //            }
        //            con.Close();
        //        }
        //    }
        //    catch (Exception err)
        //    {

        //    }
        //    return lstTix;
        //}
    }
}
