using Pustok.Enums;
using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
    public class Order
    {
        public int Id { get; set; }
        public double TotalPrice { get; set; }
        [StringLength(maximumLength:50)]
        public string FullName { get; set; }
        [StringLength(maximumLength: 50)]

        public string Country { get; set; }
        [StringLength(maximumLength: 50)]

        public string Email { get; set; }
        public string Phone { get; set; }
        [StringLength(maximumLength: 100)]

        public string Address { get; set; }
        [StringLength(maximumLength: 50)]

        public string City { get; set; }
        public DateTime dateTime { get; set; }
        public string ZipCode { get; set; }
        [StringLength(maximumLength: 150)]

        public string? Note { get; set; }
        public OrderStatus OrderStatus { set; get; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
