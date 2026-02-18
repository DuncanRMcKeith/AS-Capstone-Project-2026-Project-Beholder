namespace CapstoneProject.Models
{
    public class CharacterModel
    {
        public int Character_ID { get; set; }      // Primary key
        public int Creator_ID { get; set; } // Foreign key to user
        public string FName { get; set; } = "";
        public string LName { get; set; } = "";
        public string Title { get; set; } = "";
        public int Level { get; set; } = 0;
        public string CharacterClass { get; set; } = "";
        public int Strength { get; set; } = 0;
        public int Dexterity { get; set; } = 0;
        public int Constitution { get; set; } = 0;
        public int Intelligence { get; set; } = 0;
        public int Wisdom { get; set; } = 0;
        public int Charisma { get; set; } = 0;
        public string? Notes { get; set; } // Notes can be null
        public string Image_Path { get; set; } = ""; // path to image
        
        public int Slots { get; set; } = 0; // Number of character slots used by the creator
    }

}
