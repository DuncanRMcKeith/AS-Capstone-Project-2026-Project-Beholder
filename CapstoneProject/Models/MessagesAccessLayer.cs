using System.Data;
using Microsoft.Data.SqlClient;
using CapstoneProject.Models;

namespace CapstoneProject.Models
{
    public class MessagesAccessLayer
    {
        string? connectionString;
        private readonly IConfiguration _configuration;

        public MessagesAccessLayer(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public void SaveMessage(int sendingUserId, string content, int? receivingUserId = null, int? commId = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"INSERT INTO Messages (Sending_User, Receiving_User, Content, Sent_At, Comm_ID) 
                                   VALUES (@sendingUser, @receivingUser, @content, GETDATE(), @commId)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@sendingUser", sendingUserId);
                        cmd.Parameters.AddWithValue("@receivingUser", (object?)receivingUserId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@content", content);
                        cmd.Parameters.AddWithValue("@commId", (object?)commId ?? DBNull.Value);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("SaveMessage error: " + err.Message);
            }
        }

        public IEnumerable<MessageModel> GetCommunityMessages(int commId)
        {
            List<MessageModel> messages = new List<MessageModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT m.Message_ID, m.Content, m.Sent_At, u.Username, u.User_ID
                                   FROM Messages m
                                   INNER JOIN Users u ON m.Sending_User = u.User_ID
                                   WHERE m.Comm_ID = @commId
                                   ORDER BY m.Sent_At ASC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@commId", commId);
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            messages.Add(new MessageModel
                            {
                                Message_ID = Convert.ToInt32(reader["Message_ID"]),
                                Content = reader["Content"].ToString(),
                                Timestamp = Convert.ToDateTime(reader["Sent_At"]),
                                SenderUsername = reader["Username"].ToString(),
                                Sending_User = Convert.ToInt32(reader["User_ID"])
                            });
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("GetCommunityMessages error: " + err.Message);
            }
            return messages;
        }

        public IEnumerable<MessageModel> GetDirectMessages(int userId1, int userId2)
        {
            List<MessageModel> messages = new List<MessageModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT m.Message_ID, m.Content, m.Sent_At, u.Username, u.User_ID
                                   FROM Messages m
                                   INNER JOIN Users u ON m.Sending_User = u.User_ID
                                   WHERE (m.Sending_User = @userId1 AND m.Receiving_User = @userId2)
                                   OR (m.Sending_User = @userId2 AND m.Receiving_User = @userId1)
                                   ORDER BY m.Sent_At ASC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId1", userId1);
                        cmd.Parameters.AddWithValue("@userId2", userId2);
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            messages.Add(new MessageModel
                            {
                                Message_ID = Convert.ToInt32(reader["Message_ID"]),
                                Content = reader["Content"].ToString(),
                                Timestamp = Convert.ToDateTime(reader["Sent_At"]),
                                SenderUsername = reader["Username"].ToString(),
                                Sending_User = Convert.ToInt32(reader["User_ID"])
                            });
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("GetDirectMessages error: " + err.Message);
            }
            return messages;
        }
    }
}