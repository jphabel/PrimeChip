using System.ComponentModel.DataAnnotations;

namespace PrimeChip.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
