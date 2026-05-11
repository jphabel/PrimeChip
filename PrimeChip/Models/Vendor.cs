using System.ComponentModel.DataAnnotations;

namespace PrimeChip.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        [Required] 
       public string vendorName { get; set; }
        [Required]
        public string contactNumber { get; set; }
        [Required]
        public string address { get; set; } 

    }
}
