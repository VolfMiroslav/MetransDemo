using System.ComponentModel.DataAnnotations;

namespace MetransDemo.Models
{
    public class Status
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "Název stavu")]
        public string? Name { get; set; }
        [Required]
        [Display(Name = "Kód stavu")]
        public string? Code { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
