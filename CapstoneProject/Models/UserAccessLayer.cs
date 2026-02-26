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
                string sql = "INSERT INTO Users (Username, Email, Password, Profilepic, Created_Date) VALUES (@user, @email, @password, 'images/default.png', GETDATE())";


                
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
                                    user.User_Description = reader["User_Description"].ToString();
                                    user.Profilepic = reader["Profilepic"].ToString();
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


        public IEnumerable<UserModel> GetFriendRequests(int userID)
        {
            List<UserModel> lstreq = new List<UserModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string strsql = @"
                    SELECT u.User_ID, u.Username, u.Profilepic
                    FROM Users u
                    INNER JOIN FriendRequests f ON u.User_ID = f.Sending_User_ID
                    WHERE f.Receiving_User_ID = @UserId";
                    SqlCommand Cmd = new SqlCommand(strsql, conn);
                    Cmd.Parameters.AddWithValue("@UserId", userID);
                    Cmd.CommandType = CommandType.Text;

                    conn.Open();
                    SqlDataReader rdr = Cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        lstreq.Add(new UserModel
                        {
                            User_ID = Convert.ToInt32(rdr["User_ID"]),
                            Username = Convert.ToString(rdr["Username"]),
                            Profilepic = Convert.ToString(rdr["Profilepic"])
                        });

                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            {

            }
            return lstreq;
        }

        public IEnumerable<UserModel> GetFriends(int userID)
        {
            List<UserModel> lstusers = new List<UserModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string strsql = @"
                    SELECT u.User_ID, u.Username, u.Profilepic
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
                            Profilepic = Convert.ToString(rdr["Profilepic"])
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

        public UserModel GetUserByID(int id)
        {
            UserModel user = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Users WHERE User_ID= @id";
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new UserModel();
                                {
                                    user.Username = reader["Username"].ToString();
                                    user.Email = reader["Email"].ToString();
                                    user.User_Description = reader["User_Description"].ToString();
                                    user.Profilepic = reader["Profilepic"].ToString();
                                }
                                ;
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

        public void AddFriend(int userid, int friendid)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //add friends to table

                string sql = "INSERT INTO Friends (User1, User2) VALUES (@user1, @user2)";


                //compare 2 IDs, store smallest one as user1. helps keep table organized
                int user1;
                int user2;

                if (userid < friendid)
                {
                    user1 = userid;
                    user2 = friendid;
                }
                else
                {
                    user1 = friendid;
                    user2 = userid;
                }

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@user1", user1);
                    command.Parameters.AddWithValue("@user2", user2);


                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }
            }
        }

        public void RemoveRequest(int userid, int friendid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {


                //remove friend request from requests table

                string sql = "DELETE FROM FriendRequests WHERE Sending_User_ID = @friend AND Receiving_User_ID = @user";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@friend", friendid);
                    command.Parameters.AddWithValue("@user", userid);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void RemoveFriend(int userid, int friendid)
        {
            //sort userid and friendid like we do when we create the friendship
            int user1;
            int user2;
            if (userid < friendid)
            {
                user1 = userid;
                user2 = friendid;
            }
            else
            {
                user1 = friendid;
                user2 = userid;
            }
            using(SqlConnection connection = new SqlConnection(connectionString)) { 
                string sql = "DELETE FROM Friends WHERE User1 = @user1 AND User2 = @user2";

                using(SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@user1", user1);
                    command.Parameters.AddWithValue("@user2", user2);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                    
            }
        }

        public void AddFriendRequest(int userid, int friendid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO FriendRequests (Sending_User_ID, Receiving_User_ID) VALUES (@currentuser, @friend)";



                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@currentuser", userid);
                    command.Parameters.AddWithValue("@friend", friendid);
                    

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                }
            }
        }

        public IEnumerable<UserModel> SearchUsers(string search, int userid)
        {
            List<UserModel> lstusers = new List<UserModel>();
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT Username, Profilepic, User_ID FROM Users WHERE Username LIKE @search AND User_ID != @currentuser AND NOT EXISTS (select 1 from Friends where(Friends.User1 = Users.User_ID AND Friends.User2 = @currentuser) OR (Friends.User2 = Users.User_ID AND Friends.User1 = @currentuser))";
                try { 
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType= CommandType.Text;
                    command.Parameters.AddWithValue("@search",'%'+ search + '%');
                    command.Parameters.AddWithValue("@currentuser", userid);
                        connection.Open();
                    SqlDataReader rdr = command.ExecuteReader();

                    while (rdr.Read())
                    {
                        lstusers.Add(new UserModel
                        {
                            User_ID = Convert.ToInt32(rdr["User_ID"]),
                            Username = Convert.ToString(rdr["Username"]),
                            Profilepic = Convert.ToString(rdr["Profilepic"])
                        });

                    }
                    connection.Close();
                }
            }
            catch (Exception err)
            {

            }
            }
            return lstusers;
        }
        public void UpdateUserImage(UserModel user)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = "UPDATE Users SET Profilepic = @pic WHERE User_ID = @id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pic", user.Profilepic);
                    cmd.Parameters.AddWithValue("@id", user.User_ID);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void SaveMessage(int sendingUser, int receivingUser, string content, int? commId = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Messages (Sending_User, Receiving_User, Content, Sent_At, Comm_ID) 
                       VALUES (@sending, @receiving, @content, GETDATE(), @commId)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@sending", sendingUser);
                    command.Parameters.AddWithValue("@receiving", receivingUser);
                    command.Parameters.AddWithValue("@content", content);
                    command.Parameters.AddWithValue("@commId", commId.HasValue ? (object)commId.Value : DBNull.Value);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public IEnumerable<MessageModel> GetMessages(int user1, int user2)
        {
            List<MessageModel> messages = new List<MessageModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = @"
                SELECT m.Message_ID, m.Sending_User, m.Receiving_User, m.Content, m.Sent_At, u.Username
                FROM Messages m
                JOIN Users u ON m.Sending_User = u.User_ID
                WHERE (m.Sending_User = @user1 AND m.Receiving_User = @user2)
                   OR (m.Sending_User = @user2 AND m.Receiving_User = @user1)
                ORDER BY m.Sent_At ASC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@user1", user1);
                        command.Parameters.AddWithValue("@user2", user2);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            messages.Add(new MessageModel
                            {
                                Message_ID = Convert.ToInt32(reader["Message_ID"]),
                                Sending_User = Convert.ToInt32(reader["Sending_User"]),
                                Receiving_User = Convert.ToInt32(reader["Receiving_User"]),
                                Content = reader["Content"].ToString(),
                                Sent_At = Convert.ToDateTime(reader["Sent_At"]),
                                SenderUsername = reader["Username"].ToString()
                            });
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("ERROR: " + err.Message);
            }
            return messages;
        }
    }
}
