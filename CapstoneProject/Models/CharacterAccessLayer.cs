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
                //Checks how many characters have already been created by the user and assigns the next available slot number to the new character. If the user already has 4 characters, it throws an exception.
                        connection.Open();
                int count = CountByCreatorId(character.Creator_ID);

                        if (count >= 4)
                        {
                            throw new Exception("User already has 4 characters.");
                        }

                     character.Slots = count + 1;  // Slot will be 1, 2, 3, or 4

                string sql = "INSERT INTO Characters ( Creator_ID, FName, LName, Title, Level, Char_class, Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma, Notes, Image_Path, Slots) VALUES (@Creator_ID, @FName, @LName, @Title, @Level, @Char_class, @Strength, @Dexterity, @Constitution, @Intelligence, @Wisdom, @Charisma, @Notes, @Image_Path, @Slots)";



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
                        command.Parameters .AddWithValue("@Slots", character.Slots);

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
        public CharacterModel GetCharacterBySlot(string creatorId, int slot)
        {
            CharacterModel character = null;

            using SqlConnection conn = new SqlConnection(connectionString);
            string sql = "SELECT * FROM Characters WHERE Creator_ID = @Creator_ID AND Slots = @Slots";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Creator_ID", creatorId);
            cmd.Parameters.AddWithValue("@Slots", slot);

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                character = new CharacterModel
                {
                    Character_ID = (int)reader["Character_ID"],
                    Creator_ID = reader["Creator_ID"].ToString(),
                    FName = reader["FName"].ToString(),
                    LName = reader["LName"].ToString(),
                    Title = reader["Title"].ToString(),
                    Level = (int)reader["Level"],
                    CharacterClass = reader["Char_class"].ToString(),
                    Strength = (int)reader["Strength"],
                    Dexterity = (int)reader["Dexterity"],
                    Constitution = (int)reader["Constitution"],
                    Intelligence = (int)reader["Intelligence"],
                    Wisdom = (int)reader["Wisdom"],
                    Charisma = (int)reader["Charisma"],
                    Notes = reader["Notes"].ToString(),
                    Image_Path = reader["Image_Path"].ToString(),
                    Slots = (int)reader["Slots"]
                };
            }

            return character;
        }
    }
}
