using System.ComponentModel.DataAnnotations;
namespace PrimeChip.Models
{
    public class User
    {
        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
