using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Shopping.DataAccess.Models
{
    [Keyless]
    public class OrderItem
    {
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Product_Name { get; set; }

        [Required]
        public byte[]? Product_Image { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public int OrderId { get; set; }
    }
}
