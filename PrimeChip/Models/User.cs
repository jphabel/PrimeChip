using System.ComponentModel.DataAnnotations;
namespace PrimeChip.Models
{
    public class User
    {
        public int Id { get; set; } //primary key for the user

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
