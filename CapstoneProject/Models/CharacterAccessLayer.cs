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
                    string sql = "INSERT INTO Characters (Creator_ID, FName, LName, Title, Level, Char_class,    Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma, Notes, Image_Path) VALUES (@Creator_ID, @FName, @LName, @Title, @Level, @Char_class, @Strength, @Dexterity, @Constitution, @Intelligence, @Wisdom, @Charisma, @Notes, @Image_Path)";



                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@FName", Input.FName ?? "");
                        command.Parameters.AddWithValue("@LName", Input.LName ?? "");
                        command.Parameters.AddWithValue("@Title", Input.Title ?? "");
                        command.Parameters.AddWithValue("@Level", Input.Level);
                        command.Parameters.AddWithValue("@Char_class", Input.CharacterClass ?? "");
                        command.Parameters.AddWithValue("@Strength", Input.Strength);
                        command.Parameters.AddWithValue("@Dexterity", Input.Dexterity);
                        command.Parameters.AddWithValue("@Constitution", Input.Constitution);
                        command.Parameters.AddWithValue("@Intelligence", Input.Intelligence);
                        command.Parameters.AddWithValue("@Wisdom", Input.Wisdom);
                        command.Parameters.AddWithValue("@Charisma", Input.Charisma);
                        command.Parameters.AddWithValue("@Notes", Input.Notes ?? "");
                        command.Parameters.AddWithValue("@Image_Path", imagePath);


                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                    }
                }

            }
        }
}
