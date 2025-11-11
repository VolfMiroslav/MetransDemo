using System.ComponentModel.DataAnnotations;
using MetransDemo.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace MetransDemo.Models
{
    public class Order
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "Datum objednávky")]
        public DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = "Množství")]
        public decimal Amount { get; set; }

        [Display(Name = "Zákazník")]
        public int CustomerId { get; set; }
        [Display(Name = "Zákazník")]
        public Customer? Customer { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; }
        [Display(Name = "Status")]
        public Status? Status { get; set; }
    }
}