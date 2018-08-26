using System.ComponentModel.DataAnnotations;

namespace BosnaJezik.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Min lenght is 8 chars")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password´s must match")]
        public string ConfirmPassword { get; set; }
    }
}