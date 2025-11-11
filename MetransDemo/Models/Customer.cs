using System.ComponentModel.DataAnnotations;

namespace MetransDemo.Models
{
    public class Customer
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "Název")]
        public string? Name { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
