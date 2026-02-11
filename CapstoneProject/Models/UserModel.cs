using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public class UserModel
    {
        public int User_ID { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [ValidateNever]
        public string Feedback { get; set; } = string.Empty;
            
    }
}
