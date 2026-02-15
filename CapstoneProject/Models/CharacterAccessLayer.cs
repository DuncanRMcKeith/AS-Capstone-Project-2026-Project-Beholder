using CapstoneProject.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CapstoneProject.Models
{
    public class CharacterAccessLayer
    {
            string connectionString;
            private readonly IConfiguration _configuration;

            public CharacterAccessLayer(IConfiguration configuration)
            {
                _configuration = configuration;
                connectionString = _configuration.GetConnectionString("DefaultConnection");
            }

        // Create a new character in the database
        public void create(CharacterModel character)
            {


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = "INSERT INTO Characters ( Creator_ID, FName, LName, Title, Level, Char_class, Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma, Notes, Image_Path) VALUES (@Creator_ID, @FName, @LName, @Title, @Level, @Char_class, @Strength, @Dexterity, @Constitution, @Intelligence, @Wisdom, @Charisma, @Notes, @Image_Path)";



                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@Creator_ID", character.Creator_ID ?? "");
                        command.Parameters.AddWithValue("@FName", character.FName ?? "");
                        command.Parameters.AddWithValue("@LName", character.LName ?? "");
                        command.Parameters.AddWithValue("@Title", character.Title ?? "");
                        command.Parameters.AddWithValue("@Level", character.Level);
                        command.Parameters.AddWithValue("@Char_class", character.CharacterClass ?? "");
                        command.Parameters.AddWithValue("@Strength", character.Strength);
                        command.Parameters.AddWithValue("@Dexterity", character.Dexterity);
                        command.Parameters.AddWithValue("@Constitution", character.Constitution);
                        command.Parameters.AddWithValue("@Intelligence", character.Intelligence);
                        command.Parameters.AddWithValue("@Wisdom", character.Wisdom);
                        command.Parameters.AddWithValue("@Charisma", character.Charisma);
                        command.Parameters.AddWithValue("@Notes", character.Notes ?? "");
                        command.Parameters.AddWithValue("@Image_Path", character.Image_Path ?? "");

                        command.ExecuteNonQuery();
                        connection.Close();

                    }
                }

            }
        // Count the number of characters created by a specific user
        public int CountByCreatorId(string creatorId)
        {
            using SqlConnection conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Characters WHERE Creator_ID = @Creator_ID", conn);

            cmd.Parameters.AddWithValue("@Creator_ID", creatorId);

            conn.Open();
            return (int)cmd.ExecuteScalar();
        }
    }
}
