using System.ComponentModel.DataAnnotations;

namespace BlogStore.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is a Required field")]
        [EmailAddress(ErrorMessage = "Email must be in proper format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is a required field")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
