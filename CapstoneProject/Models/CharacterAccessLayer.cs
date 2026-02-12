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

            public void create(CharacterModel character)
            {


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = "INSERT INTO Characters ( FName, LName, Title, Level, Char_class, Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma, Notes) VALUES ( @FName, @LName, @Title, @Level, @Char_class, @Strength, @Dexterity, @Constitution, @Intelligence, @Wisdom, @Charisma, @Notes)";



                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
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
                        //command.Parameters.AddWithValue("@Image_Path", imagePath);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                    }
                }

            }
        }
}
