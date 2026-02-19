using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public class PostsModel 
    {


        public int Post_ID { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public int Creator_ID { get; set; }
        [ValidateNever]
        public int? Comm_ID { get; set; } = null;

        public int Likes { get; set; } = 0;
        public DateTime Created_At { get; set; } = DateTime.Now;
    }
}
